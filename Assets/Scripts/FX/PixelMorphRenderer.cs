//#define PROFILE_IN_HOT_LOOP
using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Profiling;


public class PixelMorphRenderer
{
    // constants
    private const ushort MaxVerts = 0xFFFF; // ushort max value
    private const int MaxTri = MaxVerts * 6 / 4;
    private const int VertsPerQuad = 4;
    private const int IndexSpaces = 8; // triangle indices are grouped into "spaces", with each space going from 0 up to max ushort
    private const int TotalVertSpace = MaxVerts * IndexSpaces;
    private const int TotalMaxTri = IndexSpaces * MaxTri;

    // required material
    private readonly Shader spriteShader; // instances created from this material
    private Texture2D blankWhiteTexture;

    // Rendering Components
    private readonly List<MeshRenderer> renderers;
    private readonly List<Mesh> meshes;
    private readonly Transform meshTransform;
    private readonly GameObject stage;
    private Vector2 viewportSize;
    private Camera renderCamera;

    private readonly List<DrawData> queuedDraws = new List<DrawData>();

    // Rendering Data
    private readonly ushort[] emptyArray = new ushort[0];
    private readonly Vector3[] vertices = new Vector3[TotalVertSpace];
    private readonly Vector2[] uvs = new Vector2[TotalVertSpace];
    //private readonly Vector3[] normals = new Vector3[TotalVertSpace];
    private readonly Color[] vertexColors = new Color[TotalVertSpace];
    private readonly ushort[] triangles = new ushort[TotalMaxTri]; // contains indices for multiple sub-meshes, wraps around 0xFFFF (max ushort)
    private readonly Dictionary<Texture2D, Material> materialCache = new Dictionary<Texture2D, Material>();
    private readonly List<SpriteBatch> batches = new List<SpriteBatch>();
    private int vertexCursor; // tracks which vertex was last added to the buffer
    private int triCursor; // tracks which index was last added to the buffer

    public PixelMorphRenderer(GameObject stage, Camera camera)
    {
        this.stage = stage;
        renderCamera = camera;
        renderers = new List<MeshRenderer>();
        meshes = new List<Mesh>();
        meshTransform = stage.transform;
        CreateBlankTexture();

        // pre-calculate indices
        for (int i = 0; i < IndexSpaces; i++)
        {
            MeshUtils.AddTrianglesForQuads(triangles, MaxVerts / VertsPerQuad, 0, MaxTri * i);
        }

        spriteShader = Shader.Find("Unlit/SpriteShader");
    }

    public Mesh newMesh()
    {
        var child = new GameObject("Mesh", new Type[] { typeof(MeshRenderer), typeof(MeshFilter) });
        child.transform.SetParent(stage.transform, false);
        var mesh = new Mesh();
        var renderer = child.GetComponent<MeshRenderer>();
        renderer.sortingOrder = renderers.Count;
        var filter = child.GetComponent<MeshFilter>();
        filter.mesh = mesh;
        renderer.materials = new Material[1];
        meshes.Add(mesh);
        renderers.Add(renderer);
        return mesh;
    }

    public void BeginFrame()
    {
        queuedDraws.Clear();
        viewportSize = Vector2.zero;
    }

    public void Draw(Sprite sprite, Vector2 position, Vector2 size, Vector2 origin, float rotation, int sortingLayer, Color color, bool sliced, Vector2 scale)
    {
        if (viewportSize == Vector2.zero) viewportSize = new Vector2(1920, 1080); // TODO
        queuedDraws.Add(new DrawData(queuedDraws.Count, sprite, position, size, origin, rotation, sortingLayer, color, sliced));
    }

    public void EndFrame()
    {
        ClearMesh();
        Profiler.BeginSample("Sort Draws");
        SortDraws();
        Profiler.EndSample();
        RebuildMeshes(queuedDraws);
        queuedDraws.Clear();
    }

    private void SortDraws()
    {
        queuedDraws.Sort((draw1, draw2) =>
        {
            if (draw1.SortingLayer > draw2.SortingLayer)
            {
                return 1;
            }
            if (draw1.SortingLayer < draw2.SortingLayer)
            {
                return -1;
            }

            // secondary sort by queued index
            return draw1.QueuedIndex - draw2.QueuedIndex;
        });
    }

    private void CreateBlankTexture()
    {
        blankWhiteTexture = new Texture2D(1, 1);
        blankWhiteTexture.SetPixels(new[] { Color.white });
        blankWhiteTexture.Apply();
    }

    private void ClearMesh()
    {
        vertexCursor = 0;
        triCursor = 0;
        batches.Clear();
    }

    /// Batch sprites, submit the batches one-at-a-time for rendering
    /// Batching breaks when a texture changes
    private void RebuildMeshes(List<DrawData> orderedSprites)
    {
        int numSprites = orderedSprites.Count;
        if (numSprites <= 0) return;
        Profiler.BeginSample("RebuildMeshes");

        // batching trackers
        Texture2D batchTexture = blankWhiteTexture;
        if (orderedSprites[0].Sprite != null) batchTexture = orderedSprites[0].Sprite.texture;

        int beginBatchCursor = 0;
        int endBatchCursor = 0;
        int baseVertex = 0;
        int vertCount = 0;

        // loop until all sprites have been drawn
        while (beginBatchCursor < numSprites)
        {
            // begin batch
            // batch will continue until:
            //  - texture changed,
            //  - out of verts,
            //  - or out of drawables
            Profiler.BeginSample("Quick loop");
            while (endBatchCursor < numSprites)
            {
                // TODO remove debug
                //if (endBatchCursor > Time.frameCount * 40) break;

                // out of vert index space, must break batch
                int verts = orderedSprites[endBatchCursor].Sliced ? VertsPerQuad * 9 : VertsPerQuad;
                if (vertCount + verts > baseVertex + MaxVerts)
                {
                    break;
                }

                bool bothTexturesAreNull = batchTexture == blankWhiteTexture && orderedSprites[endBatchCursor].Sprite == null;
                bool texturesMatch = bothTexturesAreNull || orderedSprites[endBatchCursor].Sprite.texture == batchTexture;
                if (texturesMatch)
                {
                    vertCount += verts;
                    endBatchCursor++;
                }
                else
                {
                    break; // texture changed - different textures cannot be batched together
                }
            }
            Profiler.EndSample();

            // if we got here, the batch is over

            // create the mesh data for the batch
            bool invalidIndices = beginBatchCursor >= endBatchCursor || beginBatchCursor < 0 || beginBatchCursor > orderedSprites.Count || endBatchCursor < 0 || endBatchCursor > orderedSprites.Count;
            if (!invalidIndices)
            {
                int trianglesStart = triCursor;

                // cache UVs outside for small speed up (most layers reuse images)
                Vector2 uvBottomLeft = Vector2.zero;
                Vector2 uvBottomRight = Vector2.right;
                Vector2 uvTopLeft = Vector2.up;
                Vector2 uvTopRight = Vector2.one;
                float atlasWidth = batchTexture.width;
                float atlasHeight = batchTexture.height;
                Sprite prevImage = null;
                const float z = 0; // never changes, layering handled by triangle order

                // per-sprite loop
                Profiler.BeginSample("Hot Loop?");
                for (int c = beginBatchCursor; c < endBatchCursor; c++)
                {
                    DrawData drawable = orderedSprites[c];

                    // alpha culling
                    if (drawable.Color.a < 0.001f) continue; // auto cull transparent images
                    // position culling (N/A)
                    // all drawables here are visible: off-screen culling is handled before submitting the drawable

                    // precompute these totals for reuse later
                    int quadsToAdd = drawable.Sliced ? 9 : 1;
                    int vertsToAdd = quadsToAdd * VertsPerQuad;

                    // bounds TODO
                    var rect = new Rect(drawable.Position, drawable.Size);

                    // position vertices based on the rect position data,
                    // supports sliced images or a basic quad
                    if (drawable.Sliced)
                    {
#if PROFILE_IN_HOT_LOOP
                        Profiler.BeginSample("Position 9 Sliced Vertices");
#endif
                        MeshUtils.Position9SlicedVerts(
                            vertices, vertexCursor,
                            rect, drawable.Sprite.border, z,
                            rotationDegrees: drawable.Rotation,
                            xOrigin: drawable.Origin.x, yOrigin: drawable.Origin.y);
#if PROFILE_IN_HOT_LOOP
                        Profiler.EndSample();
#endif
                    }
                    else
                    {
#if PROFILE_IN_HOT_LOOP
                        Profiler.BeginSample("Position Quad Vertices");
#endif
                        Debug.Assert(vertexCursor + 3 <= vertices.Length);
                        MeshUtils.PositionQuadVerts(
                            vertices, vertexCursor,
                            rect, z,
                            rotationDegrees: drawable.Rotation,
                            xOrigin: drawable.Origin.x, yOrigin: drawable.Origin.y);
#if PROFILE_IN_HOT_LOOP
                        Profiler.EndSample();
#endif
                    }

#if PROFILE_IN_HOT_LOOP
                    Profiler.BeginSample("AssignColors");
#endif
                    // colors
                    // supports per-cell per-layer tinting
                    Color tint = drawable.Color;
                    for (int v = 0; v < vertsToAdd; v++)
                    {
                        vertexColors[v + vertexCursor] = tint;
                    }
#if PROFILE_IN_HOT_LOOP
                    Profiler.EndSample();
                    Profiler.BeginSample("Calculate UVs");
#endif
                    // uvs
                    if (drawable.Sliced)
                    {
                        // TODO sliced UVs
                    }
                    else
                    {
                        // previous UVs are used if the Sprite reference is the same
                        // because I found that always calculating UVs is more expensive than
                        // caching the previous UVs and checking for a match.
                        // most of the time, image == prevImage because all cells tend to have the same image on a layer
                        Sprite image = drawable.Sprite;
                        if (image != prevImage)
                        {
                            // UVs are based on the sprite position within its atlas
                            // some images are not in an atlas, in which case we default to the full texture size
                            // tight-packed sprites are not supported
                            if (image != null && image.packed && image.packingMode != SpritePackingMode.Tight)
                            {
                                Rect spriteRect = image.textureRect;
                                float xMin = spriteRect.x / atlasWidth;
                                float xMax = spriteRect.xMax / atlasHeight;
                                float yMin = spriteRect.y / atlasWidth;
                                float yMax = spriteRect.yMax / atlasHeight;
                                uvBottomLeft = new Vector2(xMin, yMin);
                                uvBottomRight = new Vector2(xMax, yMin);
                                uvTopLeft = new Vector2(xMin, yMax);
                                uvTopRight = new Vector2(xMax, yMax);
                            }
                            else
                            {
                                uvBottomLeft = new Vector2(0, 0);
                                uvBottomRight = new Vector2(1, 0);
                                uvTopLeft = new Vector2(0, 1);
                                uvTopRight = new Vector2(1, 1);
                            }
                            prevImage = image;
                        }

                        uvs[0 + vertexCursor] = uvBottomLeft;
                        uvs[1 + vertexCursor] = uvBottomRight;
                        uvs[2 + vertexCursor] = uvTopLeft;
                        uvs[3 + vertexCursor] = uvTopRight;

                    }

#if PROFILE_IN_HOT_LOOP
                    Profiler.EndSample();
#endif

                    // move cursors
                    vertexCursor += vertsToAdd;
                    triCursor += quadsToAdd * 6;
                }
                Profiler.EndSample();

                // we define the batch here so that in the outer function we can process it
                batches.Add(new SpriteBatch
                {
                    TrianglesStart = trianglesStart,
                    TrianglesEnd = triCursor,
                    Material = GetMaterial(batchTexture),
                    BaseVertex = baseVertex,
                    EndVertex = vertexCursor
                });
            }
            else
            {
                UnityEngine.Debug.LogError("[GUIMesh] batch with invalid start and end indices: (" + beginBatchCursor + "," + endBatchCursor + ")");
            }

            // setup next batch
            beginBatchCursor = endBatchCursor;
            if (endBatchCursor < numSprites)
            {
                Sprite s = orderedSprites[endBatchCursor].Sprite;
                if (s != null) batchTexture = s.texture;

                // if the last batch was the last one in its index space, we need to push the vertex cursor up to match
                int nextDrawableVerts = orderedSprites[endBatchCursor].Sliced ? VertsPerQuad * 9 : VertsPerQuad;
                if (vertCount + nextDrawableVerts > baseVertex + MaxVerts)
                {
                    // add another index space
                    baseVertex += MaxVerts;
                    vertexCursor = baseVertex;
                    triCursor = 0;
                    vertCount = baseVertex;
                }
            }

            // TODO remove debug
            // if (endBatchCursor > Time.frameCount * 40) break;
        }


        Profiler.BeginSample("Clear Tris");
        // TODO REMOVE now that we are preloading
        // clear extra tris
        //for (int i = triCursor+1; i < triangles.Length; i++)
        //{
        //  triangles[i] = 0;
        //}
        Profiler.EndSample();

        Profiler.BeginSample("Create Meshes");
        if (batches.Count > meshes.Count)
        {
            var numMeshes = batches.Count - meshes.Count;
            for (int i = 0; i < numMeshes; i++)
            {
                newMesh();
            }
        }
        Profiler.EndSample();

        Profiler.BeginSample("Materials");
        // set up materials
        for (int i = 0; i < batches.Count; i++)
        {
            var renderer = renderers[i];
            var mats = renderer.materials;
            mats[0] = batches[i].Material;
            renderer.materials = mats;
            renderer.enabled = true;
        }
        // clean up references because GC
        for (int i = batches.Count; i < renderers.Count; i++)
        {
            var renderer = renderers[i];
            renderer.enabled = false;
            var mats = renderer.materials;
            mats[0] = null;
            renderer.materials = mats;
        }
        Profiler.EndSample();

        // always assign same arrays back to mesh, avoiding allocating new buffers

        // each batch gets a submesh
        Profiler.BeginSample("Submesh Generation");
        for (int i = 0; i < batches.Count; i++)
        {
            var mesh = meshes[i];
            var baseVert = batches[i].BaseVertex;
            var numVerts = batches[i].EndVertex - baseVert;
            mesh.SetTriangles(emptyArray, 0);
            mesh.SetVertices(vertices, baseVert, numVerts);
            mesh.SetColors(vertexColors, baseVert, numVerts);
            mesh.SetUVs(0, uvs, baseVert, numVerts);
            mesh.SetTriangles(triangles, batches[i].TrianglesStart, batches[i].TrianglesEnd - batches[i].TrianglesStart, 0, false, 0);
            /*if (i > 0)
            {mesh
                int end = batches[i].IndexEnd;
                StringBuilder s = new StringBuilder();
                for (int j = batches[i].IndexStart; j < end; j++)
                {
                    s.Append(triangles[j]);
                    s.Append(",");
                }

                UnityEngine.Debug.Log(s);
            }*/
        }
        Profiler.EndSample();

        // Not doing this for now
        //Profiler.BeginSample("Scale For Screen");
        //// position and scale the mesh so that the camera sees it as screen-space set mesh transform
        //const float distFromCamera = 10;
        //float worldSpaceHalfHeight = Mathf.Tan(Mathf.Deg2Rad * renderCamera.fieldOfView * .5f) * distFromCamera;
        //float scale = 2 * worldSpaceHalfHeight / viewportSize.y; //only scale on height (for now)
        //Vector3 bottomLeft = renderCamera.ViewportToWorldPoint(new Vector3(0, 0, distFromCamera));
        //Vector3 topRight = renderCamera.ViewportToWorldPoint(new Vector3(1, 1, distFromCamera));
        //float leftOffset = (topRight.x - bottomLeft.x) * .5f;
        //float bottomOffset = (topRight.y - bottomLeft.y) * .5f;
        //meshTransform.localScale = scale * Vector3.one;
        //meshTransform.position = renderCamera.transform.position + new Vector3(-leftOffset, -bottomOffset, distFromCamera);
        //Profiler.EndSample();

        Profiler.EndSample();
    }

    private Material GetMaterial(Texture2D texture)
    {
        if (materialCache.ContainsKey(texture))
        {
            return materialCache[texture];
        }

        var mat = new Material(spriteShader) { mainTexture = texture };
        materialCache[texture] = mat;
        return mat;
    }

    private struct DrawData
    {
        // ReSharper disable FieldCanBeMadeReadOnly.Local
        // ReSharper disable MemberCanBePrivate.Local
        public Sprite Sprite; // 8
        public Vector2 Position; // 16
        public Vector2 Size; // 24
        public Vector2 Origin; // 32
        public float Rotation; // 36
        public int SortingLayer; // 40
        public Color Color; // 56
        public int QueuedIndex; // 60
        public bool Sliced; // 61 + 3 pad = 64
        // ReSharper restore FieldCanBeMadeReadOnly.Local
        // ReSharper restore MemberCanBePrivate.Local

        public DrawData(int queuedIndex, Sprite sprite, Vector2 position, Vector2 size, Vector2 origin, float rotation, int sortingLayer, Color color, bool sliced)
        {
            QueuedIndex = queuedIndex;
            Sprite = sprite;
            Position = position;
            Size = size;
            Origin = origin;
            Rotation = rotation;
            SortingLayer = sortingLayer;
            Color = color;
            Sliced = sliced;
        }
    }

    private struct SpriteBatch
    {
        public int TrianglesStart;
        public int TrianglesEnd;
        public Material Material;
        public int BaseVertex; // indices can wrap around, they use BaseVertex to
        public int EndVertex;
    }
}