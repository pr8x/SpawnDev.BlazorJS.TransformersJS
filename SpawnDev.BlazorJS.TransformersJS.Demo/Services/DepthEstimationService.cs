using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS.Demo.Services
{
    public class DepthEstimationService
    {
        Pipelines? Pipelines = null;
        public Dictionary<string, ModelLoadProgress> ModelProgresses { get; } = new();
        ActionCallback<ModelLoadProgress> OnProgress => new ActionCallback<ModelLoadProgress>(Pipeline_OnProgress);
        Dictionary<string, DepthEstimationPipeline> DepthEstimationPipelines = new Dictionary<string, DepthEstimationPipeline>();
        public DepthEstimationService()
        {

        }
        void Log(string msg, bool newLine = true)
        {
            Console.WriteLine(msg);
        }
        public event Action OnStateChange = default!;
        void StateHasChanged()
        {
            OnStateChange?.Invoke();
        }
        void Pipeline_OnProgress(ModelLoadProgress obj)
        {
            if (!string.IsNullOrEmpty(obj.File))
            {
                if (ModelProgresses.TryGetValue(obj.File, out var progress))
                {
                    progress.Status = obj.Status;
                    if (obj.Progress != null) progress.Progress = obj.Progress;
                    if (obj.Total != null) progress.Total = obj.Total;
                    if (obj.Loaded != null) progress.Loaded = obj.Loaded;
                }
                else
                {
                    ModelProgresses[obj.File] = obj;
                }
            }
            StateHasChanged();
        }
        SemaphoreSlim LoadLimiter = new SemaphoreSlim(1);
        public async Task<DepthEstimationPipeline> GetDepthEstimationPipeline(string model = "onnx-community/depth-anything-v2-small", bool useWebGPU = true)
        {
            if (string.IsNullOrEmpty(model))
            {
                model = "onnx-community/depth-anything-v2-small";
            }
            var key = useWebGPU ? $"{model}+webgpu" : model;
            if (DepthEstimationPipelines.TryGetValue(key, out var depthEstimationPipeline))
            {
                return depthEstimationPipeline;
            }
            await LoadLimiter.WaitAsync();
            try
            {
                if (Pipelines == null)
                {
                    Log($"Initializing... ", false);
                    Pipelines = await Pipelines.Init();
                    Log($"Done");
                }
                Log($"Depth Estimation Pipeline with WebGPU loading... ", false);
                depthEstimationPipeline = await Pipelines.DepthEstimationPipeline(model, new PipelineOptions { Device = useWebGPU ? "webgpu" : null, OnProgress = OnProgress });
                DepthEstimationPipelines[key] = depthEstimationPipeline;
                Log($"Done");
                ModelProgresses.Clear();
                StateHasChanged();
                return depthEstimationPipeline;
            }
            finally
            {
                LoadLimiter.Release();
            }
        }
        Dictionary<string, HTMLImageElement> Results = new Dictionary<string, HTMLImageElement>();
        public async Task<HTMLImageElement> ImageTo2DZImage(HTMLImageElement image, string model = "onnx-community/depth-anything-v2-small", bool useWebGPU = true)
        {
            var source = image.Src;
            if (!Results.TryGetValue(source, out var imageWithDepth))
            {
                // get the depth estimation pipeline
                var DepthEstimationPipeline = await GetDepthEstimationPipeline(model, useWebGPU);
                // generate the depth map
                using var depthResult = await DepthEstimationPipeline!.Call(source);
                using var depthInfo = depthResult.Depth;
                using var depthMapData = depthInfo.Data;
                var depthWidth = depthInfo.Width;
                var depthHeight = depthInfo.Height;
                // create 2D+Z image object url
                var imageWithDepthObjectUrl = await Create2DZObjectUrl(image, depthMapData, depthWidth, depthHeight);
                imageWithDepth = await HTMLImageElement.CreateFromImageAsync(imageWithDepthObjectUrl);
                Results[source] = imageWithDepth;
            }
            return imageWithDepth;
        }
        public async Task<HTMLImageElement> ImageTo2DZImage(string source, string model = "onnx-community/depth-anything-v2-small", bool useWebGPU = true)
        {
            if (!Results.TryGetValue(source, out var imageWithDepth))
            {
                // get image
                using var image = await HTMLImageElement.CreateFromImageAsync(source);
                // get the depth estimation pipeline
                var DepthEstimationPipeline = await GetDepthEstimationPipeline(model, useWebGPU);
                // generate the depth map
                using var depthResult = await DepthEstimationPipeline!.Call(source);
                using var depthInfo = depthResult.Depth;
                using var depthMapData = depthInfo.Data;
                var depthWidth = depthInfo.Width;
                var depthHeight = depthInfo.Height;
                // create 2D+Z image object url
                var imageWithDepthObjectUrl = await Create2DZObjectUrl(image, depthMapData, depthWidth, depthHeight);
                imageWithDepth = await HTMLImageElement.CreateFromImageAsync(imageWithDepthObjectUrl);
                Results[source] = imageWithDepth;
            }
            return imageWithDepth;
        }
        async Task<string> Create2DZObjectUrl(HTMLImageElement rgbImage, Uint8Array grayscale1BPPData, int width, int height)
        {
            var outWidth = width * 2;
            var outHeight = height;
            var grayscaleDataBytes = grayscale1BPPData.ReadBytes();
            var depthmapRGBABytes = Grayscale1BPPToRGBA(grayscaleDataBytes, width, height);
            using var canvas = new HTMLCanvasElement(outWidth, outHeight);
            using var ctx = canvas.Get2DContext();
            // draw rgb image
            ctx.DrawImage(rgbImage);
            // draw depth map
            ctx.PutImageBytes(depthmapRGBABytes, width, height, width, 0);
            using var blob = await canvas.ToBlobAsync("image/png");
            var ret = URL.CreateObjectURL(blob);
            return ret;
        }
        async Task<string> CreateDepthImageObjectUrl(Uint8Array grayscale1BPPData, int width, int height)
        {
            var grayscaleDataBytes = grayscale1BPPData.ReadBytes();
            var depthmapRGBABytes = Grayscale1BPPToRGBA(grayscaleDataBytes, width, height);
            using var canvas = new HTMLCanvasElement(width, height);
            using var ctx = canvas.Get2DContext();
            ctx.PutImageBytes(depthmapRGBABytes, width, height);
            using var blob = await canvas.ToBlobAsync("image/png");
            var ret = URL.CreateObjectURL(blob);
            return ret;
        }
        byte[] Grayscale1BPPToRGBA(byte[] grayscaleData, int width, int height)
        {
            var ret = new byte[width * height * 4];
            for (var i = 0; i < grayscaleData.Length; i++)
            {
                var grayValue = grayscaleData[i];
                ret[i * 4] = grayValue;     // Red
                ret[i * 4 + 1] = grayValue; // Green
                ret[i * 4 + 2] = grayValue; // Blue
                ret[i * 4 + 3] = 255;       // Alpha
            }
            return ret;
        }
    }
}
