using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS.Demo.Renderers
{
    public class AnaglyphProfile
    {
        public string Name { get; set; } = "";
        public float[] Data { get; set; } = null;
    }
    public class AnaglyphRenderer : MultiviewRenderer
    {
        int _ProfileIndex = 0;
        public int ProfileIndex
        {
            get => _ProfileIndex;
            set
            {
                if (value >= 0 && value < AnaglyphProfiles.Count)
                {
                    _ProfileIndex = value;
                }
            }
        }
        public AnaglyphProfile ActiveProfile
        {
            get => AnaglyphProfiles[ProfileIndex];
        }
        List<AnaglyphProfile> AnaglyphProfiles = new List<AnaglyphProfile>()
        {
            new AnaglyphProfile{
                Name = "Green Magenta",
                Data = new float[]{
                    0.0f,
                    0.0f,
                    0.0f,
                    -0.062f,
                    -0.158f,
                    -0.039f,
                    0.529f,
                    0.705f,
                    0.024f,
                    0.284f,
                    0.668f,
                    0.143f,
                    -0.016f,
                    -0.015f,
                    -0.065f,
                    -0.015f,
                    -0.027f,
                    0.021f,
                    0.009f,
                    0.075f,
                    0.937f
                }
            },
            new AnaglyphProfile{
                Name = "Red Cyan",
                Data = new float[]{
                    0.0f,
                    0.0f,
                    0.0f,
                    0.456f,
                    0.500f,
                    0.176f,
                    -0.043f,
                    -0.088f,
                    -0.002f,
                    -0.040f,
                    -0.038f,
                    -0.016f,
                    0.378f,
                    0.734f,
                    -0.018f,
                    -0.015f,
                    -0.021f,
                    -0.005f,
                    -0.072f,
                    -0.113f,
                    1.226f
                }
            }
        };
        public AnaglyphRenderer() : base()
        {
            Init();
        }
        public AnaglyphRenderer(HTMLCanvasElement canvas) : base(canvas)
        {
            Init();
        }
        void Init()
        {
            var vertexShader = EmbeddedShaderLoader.GetShaderString("multiview.vertex.glsl");
            if (string.IsNullOrEmpty(vertexShader))
            {
                throw new Exception("Vertex shader not found");
            }
            var fragmentShader = EmbeddedShaderLoader.GetShaderString("anaglyph.fragment.glsl");
            if (string.IsNullOrEmpty(fragmentShader))
            {
                throw new Exception("Fragment shader not found");
            }
            program = CreateProgram(vertexShader, fragmentShader);
        }
        public override void ApplyEffect()
        {
            var profile = AnaglyphProfiles[ProfileIndex];
            Uniform1fv("agdata", profile.Data);
        }
    }
}
