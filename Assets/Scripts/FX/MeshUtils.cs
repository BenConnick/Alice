using System.Runtime.CompilerServices;
using UnityEngine;

public static class MeshUtils
{
    // ReSharper disable once IdentifierTypo
    private const ushort vertsPerQuad = 4;
    private const int vertsPerSliced = 9 * vertsPerQuad;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Position9SlicedVerts(
        Vector3[] vertexBuffer, int vertexStartingIndex,
        Rect rect,
        Vector4 borders,
        float z,
        float scaleX = 1f, float scaleY = 1f,
        float rotationDegrees = 0,
        float xOrigin = 0, float yOrigin = 0)
    {
        Position9SlicedVerts(
            vertexBuffer,
            vertexStartingIndex,
            rect.xMin, rect.yMin, rect.xMax, rect.yMax,
            borders.x, borders.y, borders.z, borders.w,
            z,
            scaleX, scaleY,
            rotationDegrees,
            xOrigin, yOrigin);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Position9SlicedVerts(
        Vector3[] vertexBuffer, int vertexStartingIndex,
        float xMin, float yMin, float xMax, float yMax,
        float leftPad, float topPad, float rightPad, float bottomPad,
        float z,
        float scaleX, float scaleY,
        float rotationDegrees,
        float xOrigin, float yOrigin)
    {
        // custom mesh is made of 9 quads with no shared vertices
        // like a classic nine-slice (think Rubix cube)

        // vertices in a quads laid out like this
        // 2 3
        // 0 1

        // quads are indexed like this
        // 0 1 2
        // 3 4 5
        // 6 7 8

        // points
        float leftBound = xMin;
        float rightBound = xMax;
        float topBound = yMax;
        float bottomBound = yMin;
        float innerLeftBound = leftBound + leftPad;
        float innerRightBound = rightBound - rightPad;
        float innerTopBound = topBound - topPad;
        float innerBottomBound = bottomBound + bottomPad;

        // quad 0
        vertexBuffer[vertexStartingIndex + 0] = new Vector3(leftBound, innerTopBound, z);
        vertexBuffer[vertexStartingIndex + 1] = new Vector3(innerLeftBound, innerTopBound, z);
        vertexBuffer[vertexStartingIndex + 2] = new Vector3(leftBound, topBound, z);
        vertexBuffer[vertexStartingIndex + 3] = new Vector3(innerLeftBound, topBound, z);

        // quad 1
        vertexBuffer[vertexStartingIndex + 4] = new Vector3(innerLeftBound, innerTopBound, z);
        vertexBuffer[vertexStartingIndex + 5] = new Vector3(innerRightBound, innerTopBound, z);
        vertexBuffer[vertexStartingIndex + 6] = new Vector3(innerLeftBound, topBound, z);
        vertexBuffer[vertexStartingIndex + 7] = new Vector3(innerRightBound, topBound, z);

        // quad 2
        vertexBuffer[vertexStartingIndex + 8] = new Vector3(innerRightBound, innerTopBound, z);
        vertexBuffer[vertexStartingIndex + 9] = new Vector3(rightBound, innerTopBound, z);
        vertexBuffer[vertexStartingIndex + 10] = new Vector3(innerRightBound, topBound, z);
        vertexBuffer[vertexStartingIndex + 11] = new Vector3(rightBound, topBound, z);

        // quad 3
        vertexBuffer[vertexStartingIndex + 12] = new Vector3(leftBound, innerBottomBound, z);
        vertexBuffer[vertexStartingIndex + 13] = new Vector3(innerLeftBound, innerBottomBound, z);
        vertexBuffer[vertexStartingIndex + 14] = new Vector3(leftBound, innerTopBound, z);
        vertexBuffer[vertexStartingIndex + 15] = new Vector3(innerLeftBound, innerTopBound, z);

        // quad 4
        vertexBuffer[vertexStartingIndex + 16] = new Vector3(innerLeftBound, innerBottomBound, z);
        vertexBuffer[vertexStartingIndex + 17] = new Vector3(innerRightBound, innerBottomBound, z);
        vertexBuffer[vertexStartingIndex + 18] = new Vector3(innerLeftBound, innerTopBound, z);
        vertexBuffer[vertexStartingIndex + 19] = new Vector3(innerRightBound, innerTopBound, z);

        // quad 5
        vertexBuffer[vertexStartingIndex + 20] = new Vector3(innerRightBound, innerBottomBound, z);
        vertexBuffer[vertexStartingIndex + 21] = new Vector3(rightBound, innerBottomBound, z);
        vertexBuffer[vertexStartingIndex + 22] = new Vector3(innerRightBound, innerTopBound, z);
        vertexBuffer[vertexStartingIndex + 23] = new Vector3(rightBound, innerTopBound, z);

        // quad 6
        vertexBuffer[vertexStartingIndex + 24] = new Vector3(leftBound, bottomBound, z);
        vertexBuffer[vertexStartingIndex + 25] = new Vector3(innerLeftBound, bottomBound, z);
        vertexBuffer[vertexStartingIndex + 26] = new Vector3(leftBound, innerBottomBound, z);
        vertexBuffer[vertexStartingIndex + 27] = new Vector3(innerLeftBound, innerBottomBound, z);

        // quad 7
        vertexBuffer[vertexStartingIndex + 28] = new Vector3(innerLeftBound, bottomBound, z);
        vertexBuffer[vertexStartingIndex + 29] = new Vector3(innerRightBound, bottomBound, z);
        vertexBuffer[vertexStartingIndex + 30] = new Vector3(innerLeftBound, innerBottomBound, z);
        vertexBuffer[vertexStartingIndex + 31] = new Vector3(innerRightBound, innerBottomBound, z);

        // quad 8
        vertexBuffer[vertexStartingIndex + 32] = new Vector3(innerRightBound, bottomBound, z);
        vertexBuffer[vertexStartingIndex + 33] = new Vector3(rightBound, bottomBound, z);
        vertexBuffer[vertexStartingIndex + 34] = new Vector3(innerRightBound, innerBottomBound, z);
        vertexBuffer[vertexStartingIndex + 35] = new Vector3(rightBound, innerBottomBound, z);

        // Optional Scaling
        // ReSharper disable twice CompareOfFloatsByEqualityOperator
        if (scaleX != 1f || scaleY != 1f)
        {
            ScaleVerts(vertexBuffer, vertexStartingIndex, vertsPerSliced, scaleX, scaleY, xOrigin, yOrigin);
        }

        // Optional Rotation
        if (rotationDegrees != 0)
        {
            RotateVerts(vertexBuffer, vertexStartingIndex, vertsPerSliced, rotationDegrees, xOrigin, yOrigin);
        }

        // normals, triangles, and colors handled outside
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void PositionQuadVerts(Vector3[] vertexBuffer, int vertexStartingIndex, Rect rect, float z, float scaleX = 1f, float scaleY = 1f, float rotationDegrees = 0, float xOrigin = 0, float yOrigin = 0)
    {
        vertexBuffer[vertexStartingIndex + 0] = new Vector3(rect.xMin, rect.yMin, z);
        vertexBuffer[vertexStartingIndex + 1] = new Vector3(rect.xMax, rect.yMin, z);
        vertexBuffer[vertexStartingIndex + 2] = new Vector3(rect.xMin, rect.yMax, z);
        vertexBuffer[vertexStartingIndex + 3] = new Vector3(rect.xMax, rect.yMax, z);
        // impl matches other signature, but inlines instead of calling in case the compiler doesn't inline

        // Optional Scaling
        // ReSharper disable twice CompareOfFloatsByEqualityOperator
        if (scaleX != 1f || scaleY != 1f)
        {
            ScaleVerts(vertexBuffer, vertexStartingIndex, vertsPerQuad, scaleX, scaleY, xOrigin, yOrigin);
        }

        // Optional Rotation
        if (rotationDegrees != 0)
        {
            RotateVerts(vertexBuffer, vertexStartingIndex, vertsPerQuad, rotationDegrees, xOrigin, yOrigin);
        }

        // normals, triangles, and colors handled outside
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ScaleVerts(Vector3[] vertexBuffer, int startIndex, int length, float scaleX, float scaleY, float xOrigin, float yOrigin)
    {
        int endIndex = startIndex + length;
        for (int i = startIndex; i < endIndex; i++)
        {
            Vector3 origin = new Vector2(xOrigin, yOrigin);
            Vector3 relative = vertexBuffer[i] - origin;
            Vector3 scaled = new Vector3(relative.x * scaleX, relative.y * scaleY, relative.z);
            vertexBuffer[i] = scaled + origin;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void RotateVerts(Vector3[] vertexBuffer, int startIndex, int length, float rotationDegrees, float xOrigin, float yOrigin)
    {
        int endIndex = startIndex + length;
        for (int i = startIndex; i < endIndex; i++)
        {
            Vector3 origin = new Vector2(xOrigin, yOrigin);
            Vector3 relative = vertexBuffer[i] - origin;
            float sin = Trig.Sin(rotationDegrees);
            float cos = Trig.Cos(rotationDegrees);
            float x1 = relative.x * cos - relative.y * sin;
            float y1 = relative.y * cos + relative.x * sin;
            vertexBuffer[i] = new Vector3(x1, y1, relative.z);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddTrianglesForQuads(ushort[] triangles, uint quadsToAdd, int vertexCursor, int triCursor)
    {
        for (int idx = 0; idx < quadsToAdd; idx++)
        {
            int vertexStartingIndex = vertexCursor + vertsPerQuad * idx;
            triangles[triCursor] = (ushort)(vertexStartingIndex + 0);
            triangles[triCursor + 1] = (ushort)(vertexStartingIndex + 2);
            triangles[triCursor + 2] = (ushort)(vertexStartingIndex + 1);
            triangles[triCursor + 3] = (ushort)(vertexStartingIndex + 2);
            triangles[triCursor + 4] = (ushort)(vertexStartingIndex + 3);
            triangles[triCursor + 5] = (ushort)(vertexStartingIndex + 1);
            triCursor += 6;
        }
    }
}

