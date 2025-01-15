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
        //public async Task<string> From2DImage(string source, string model = "onnx-community/depth-anything-v2-small", bool useWebGPU = true, float focusDepth = 0.5f, float level3D = 1)
        //{
        //    if (!Results.TryGetValue(source, out var imageWithDepth))
        //    {
        //        // get image
        //        using var image = await HTMLImageElement.CreateFromImageAsync(source);
        //        // get the depth estimation pipeline
        //        var DepthEstimationPipeline = await DepthEstimationService.GetDepthEstimationPipeline(model, useWebGPU);
        //        // generate the depth map
        //        using var depthResult = await DepthEstimationPipeline!.Call(source);
        //        using var depthInfo = depthResult.Depth;
        //        using var depthMapData = depthInfo.Data;
        //        var depthWidth = depthInfo.Width;
        //        var depthHeight = depthInfo.Height;
        //        // create 2D+Z image object url
        //        var imageWithDepthObjectUrl = await Create2DZObjectUrl(image, depthMapData, depthWidth, depthHeight);
        //        imageWithDepth = await HTMLImageElement.CreateFromImageAsync(imageWithDepthObjectUrl);
        //        Results[source] = imageWithDepth;
        //    }
        //    // use WebGL to convert 2D+Z to anaglyph
        //    anaglyphRenderer ??= new AnaglyphRenderer();
        //    anaglyphRenderer.Level3D = level3D;
        //    anaglyphRenderer.Focus3D = focusDepth;
        //    var temp = await anaglyphRenderer.Render(imageWithDepth);
        //    return temp;
        //}
        public override void ApplyEffect()
        {
            var profile = AnaglyphProfiles[ProfileIndex];
            Uniform1fv("agdata", profile.Data);
        }
    }
}
