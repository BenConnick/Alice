// StableFluids - A GPU implementation of Jos Stam's Stable Fluids on Unity
// https://github.com/keijiro/StableFluids

using UnityEngine;

namespace StableFluids
{
    public class SmokeRendering : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] int _resolution = 512;
        [SerializeField] float _viscosity = 1e-6f;
        [SerializeField] float _force = 300;
        [SerializeField] float _exponent = 200;
        [SerializeField] Texture2D _initial;
        [SerializeField] Texture2D _globalVelocity;
        [SerializeField] RenderTexture _bottomEdge;
        [SerializeField] Material _alphaBlendMat;
        public Transform stirrer;
        public Transform billboard;
        public const float FixedTimeInterval = 0.017f;

        private Vector3 previousPos;
        private int updateQueue; // used to lock the number of compute cycles to the number of forcedUpdates
        private float cumulativeTime = 0;
        private int updateCount;
        private int drawCount;

        #endregion

        #region Internal resources

        [SerializeField, HideInInspector] ComputeShader _compute;
        [SerializeField, HideInInspector] Shader _shader;

        #endregion

        #region Private members

        Material _shaderSheet;
        Vector2 _previousInput;

        static class Kernels
        {
            public const int Advect = 0;
            public const int Force = 1;
            public const int PSetup = 2;
            public const int PFinish = 3;
            public const int Jacobi1 = 4;
            public const int Jacobi2 = 5;
        }

        int ThreadCountX { get { return (_resolution                                + 7) / 8; } }
        int ThreadCountY { get { return (_resolution * Screen.height / Screen.width + 7) / 8; } }

        int ResolutionX { get { return ThreadCountX * 8; } }
        int ResolutionY { get { return ThreadCountY * 8; } }

        // Vector field buffers
        static class VFB
        {
            public static RenderTexture V1;
            public static RenderTexture V2;
            public static RenderTexture V3;
            public static RenderTexture P1;
            public static RenderTexture P2;
            public static RenderTexture dummy;
        }

        // Color buffers (for double buffering)
        RenderTexture _colorRT1;
        RenderTexture _colorRT2;

        RenderTexture AllocateBuffer(int componentCount, int width = 0, int height = 0)
        {
            var format = RenderTextureFormat.ARGBHalf;
            if (componentCount == 1) format = RenderTextureFormat.RHalf;
            if (componentCount == 2) format = RenderTextureFormat.RGHalf;

            if (width  == 0) width  = ResolutionX;
            if (height == 0) height = ResolutionY;

            var rt = new RenderTexture(width, height, 0, format);
            rt.enableRandomWrite = true;
            rt.Create();
            return rt;
        }

        #endregion

        #region MonoBehaviour implementation

        void OnValidate()
        {
            _resolution = Mathf.Max(_resolution, 8);
        }

        void Start()
        {
            _shaderSheet = new Material(_shader);

            VFB.V1 = AllocateBuffer(2);
            VFB.V2 = AllocateBuffer(2);
            VFB.V3 = AllocateBuffer(2);
            VFB.P1 = AllocateBuffer(1);
            VFB.P2 = AllocateBuffer(1);
            VFB.dummy = AllocateBuffer(2);

            _colorRT1 = AllocateBuffer(4, Screen.width, Screen.height);
            _colorRT2 = AllocateBuffer(4, Screen.width, Screen.height);

            Graphics.Blit(_initial, _colorRT1);
            if (stirrer == null)
                previousPos = Vector3.zero;
            else
                previousPos = stirrer.position;

#if UNITY_IOS
            Application.targetFrameRate = 60;
#endif
        }

        void OnDestroy()
        {
            Destroy(_shaderSheet);

            Destroy(VFB.V1);
            Destroy(VFB.V2);
            Destroy(VFB.V3);
            Destroy(VFB.P1);
            Destroy(VFB.P2);
            Destroy(VFB.dummy);

            Destroy(_colorRT1);
            Destroy(_colorRT2);
        }

        private static Texture2D toTexture2D(RenderTexture rTex)
        {
            Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
            // ReadPixels looks at the active RenderTexture.
            var prev = RenderTexture.active;
            RenderTexture.active = rTex;
            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();
            RenderTexture.active = prev;
            return tex;
        }

        public void DriveWithGameplay()
        {
            ForcedUpdate();
        }

        void ForcedUpdate()
        {
            updateQueue++;
            cumulativeTime += FixedTimeInterval;

            var dt = FixedTimeInterval;
            var dx = 1.0f / ResolutionY;

            //if (Time.frameCount == 10)
            //{
            //    var target = VFB.V1;
            //    Graphics.Blit(toTexture2D(rt), _colorRT1);
            //    RenderTexture.active = target;
            //    Texture2D td = new Texture2D(target.width, target.height);
            //    td.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
            //    //Color[] overwrite = new Color[target.width * target.height];
            //    for (int x = 0; x < target.width; x++)
            //    {
            //        for (int y = 0; y < target.height; y++)
            //        {
            //            //overwrite[x * target.height + y] = new Color(100, 0, 0, 0);
            //            td.SetPixel(x, y, new Color(1, 2, 3, 4));
            //        }
            //    }
            //    //td.SetPixels(0, 0, target.width, target.height, overwrite);
            //    td.Apply();
            //}

            // Input point in normalized position
            //var input = new Vector2(
            //    (Input.mousePosition.x - Screen.width  * 0.5f) / Screen.height,
            //    (Input.mousePosition.y - Screen.height * 0.5f) / Screen.height
            //);
            Vector3 stirrerPos = Vector3.zero;
            if (stirrer != null) stirrerPos = stirrer.position;
            Vector3 stirrerRelativePos = stirrerPos - billboard.position; // center to center
            float billboardHeight = billboard.localScale.y;
            float billboardWidth = billboard.localScale.x;
            Vector2 stirrerNormalized = new Vector3(stirrerRelativePos.x / billboardHeight, stirrerRelativePos.y / billboardHeight);
                //new Vector2(
                //(stirrerRelativePos.x - billboardWidth * .5f) / billboardWidth,
                //(stirrerRelativePos.y - billboardHeight * .5f) / billboardHeight);
            var input = stirrerNormalized;

            // Common variables
            _compute.SetFloat("Time", cumulativeTime);
            _compute.SetFloat("DeltaTime", dt);

            // Advection
            _compute.SetTexture(Kernels.Advect, "U_in", VFB.V1);
            _compute.SetTexture(Kernels.Advect, "W_out", VFB.V2);
            _compute.Dispatch(Kernels.Advect, ThreadCountX, ThreadCountY, 1);

            // Diffuse setup
            var dif_alpha = dx * dx / (_viscosity * dt);
            _compute.SetFloat("Alpha", dif_alpha);
            _compute.SetFloat("Beta", 4 + dif_alpha);
            Graphics.CopyTexture(VFB.V2, VFB.V1);
            _compute.SetTexture(Kernels.Jacobi2, "B2_in", VFB.V1);

            // Jacobi iteration
            for (var i = 0; i < 20; i++)
            {
                _compute.SetTexture(Kernels.Jacobi2, "X2_in", VFB.V2);
                _compute.SetTexture(Kernels.Jacobi2, "X2_out", VFB.V3);
                _compute.Dispatch(Kernels.Jacobi2, ThreadCountX, ThreadCountY, 1);

                _compute.SetTexture(Kernels.Jacobi2, "X2_in", VFB.V3);
                _compute.SetTexture(Kernels.Jacobi2, "X2_out", VFB.V2);
                _compute.Dispatch(Kernels.Jacobi2, ThreadCountX, ThreadCountY, 1);
            }

            // Add external force
            _compute.SetVector("ForceOrigin", input);
            _compute.SetFloat("ForceExponent", _exponent);
            _compute.SetTexture(Kernels.Force, "W_in", VFB.V2);
            _compute.SetTexture(Kernels.Force, "W_out", VFB.V3);

            //if (Input.GetMouseButton(1))
            //    // Random push
            //    _compute.SetVector("ForceVector", Random.insideUnitCircle * _force * 0.025f);
            //else if (Input.GetMouseButton(0))
            //    // Mouse drag
            //    _compute.SetVector("ForceVector", (input - _previousInput) * _force);
            //else
            //    _compute.SetVector("ForceVector", Vector4.zero);
            if (stirrer != null)
            {
                Vector3 stirrerMovement = stirrerRelativePos - previousPos;
                previousPos = stirrerRelativePos;
                Vector2 normalizedStirrerMovement = new Vector2(stirrerMovement.x / billboardWidth, stirrerMovement.y / billboardHeight);
                _compute.SetVector("ForceVector", normalizedStirrerMovement * _force);
                _compute.Dispatch(Kernels.Force, ThreadCountX, ThreadCountY, 1);
            }

            // Projection setup
            _compute.SetTexture(Kernels.PSetup, "W_in", VFB.V3);
            _compute.SetTexture(Kernels.PSetup, "DivW_out", VFB.V2);
            _compute.SetTexture(Kernels.PSetup, "P_out", VFB.P1);
            _compute.Dispatch(Kernels.PSetup, ThreadCountX, ThreadCountY, 1);

            // Jacobi iteration
            _compute.SetFloat("Alpha", -dx * dx);
            _compute.SetFloat("Beta", 4);
            _compute.SetTexture(Kernels.Jacobi1, "B1_in", VFB.V2);

            for (var i = 0; i < 20; i++)
            {
                _compute.SetTexture(Kernels.Jacobi1, "X1_in", VFB.P1);
                _compute.SetTexture(Kernels.Jacobi1, "X1_out", VFB.P2);
                _compute.Dispatch(Kernels.Jacobi1, ThreadCountX, ThreadCountY, 1);

                _compute.SetTexture(Kernels.Jacobi1, "X1_in", VFB.P2);
                _compute.SetTexture(Kernels.Jacobi1, "X1_out", VFB.P1);
                _compute.Dispatch(Kernels.Jacobi1, ThreadCountX, ThreadCountY, 1);
            }

            // Projection finish
            _compute.SetTexture(Kernels.PFinish, "W_in", VFB.V3);
            _compute.SetTexture(Kernels.PFinish, "P_in", VFB.P1);
            _compute.SetTexture(Kernels.PFinish, "U_out", VFB.V1);
            _compute.Dispatch(Kernels.PFinish, ThreadCountX, ThreadCountY, 1);

            // Apply the velocity field to the color buffer.
            var offs = Vector2.one * (Input.GetMouseButton(1) ? 0 : 1e+7f);
            _shaderSheet.SetVector("_ForceOrigin", input + offs);
            _shaderSheet.SetFloat("_ForceExponent", _exponent);
            _shaderSheet.SetTexture("_VelocityField", VFB.V1);
            Graphics.Blit(_colorRT1, _colorRT2, _shaderSheet, 0);

            // Swap the color buffers.
            var temp = _colorRT1;
            _colorRT1 = _colorRT2;
            _colorRT2 = temp;

            _previousInput = input;

            updateCount++;
            //Debug.Log("Update count: " + updateCount);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            drawCount++;
            //Debug.Log("Draw count: " + drawCount);
            if (updateQueue > 0)
            {
                // add valocity
                Graphics.Blit(_globalVelocity, VFB.V1, _alphaBlendMat);

                // add 'static' (not moving) game graphics to bottom
                Graphics.Blit(_bottomEdge, _colorRT1, _alphaBlendMat);

                // draw to camera output
                Graphics.Blit(_colorRT1, destination, _shaderSheet, 1);
                updateQueue--;
            }
            else
            {
                //Debug.Log("Outofsync");
            }
        }

        #endregion
    }
}
