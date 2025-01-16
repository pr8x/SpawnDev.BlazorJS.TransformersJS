using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS.Demo.Services
{
    public class DepthEstimationService
    {
        /// <summary>
        /// The progress percentage from 0 to 100
        /// </summary>
        public float OverallLoadProgress
        {
            get
            {
                var total = (float)ModelProgresses.Values.Sum(p => p.Total ?? 0);
                if (total == 0f) return 0;
                var loaded = (float)ModelProgresses.Values.Sum(p => p.Loaded ?? 0);
                return loaded * 100f / total;
            }
        }
        /// <summary>
        /// True if loading models
        /// </summary>
        public bool Loading { get; private set; }
        /// <summary>
        /// True if loading models
        /// </summary>
        public bool ModelsLoaded => DepthEstimationPipelines.Any();
        /// <summary>
        /// Holds the loading progress for models that are loading
        /// </summary>
        public Dictionary<string, ModelLoadProgress> ModelProgresses { get; } = new();
        /// <summary>
        /// Holds all loaded depth estimation pipelines
        /// </summary>
        public Dictionary<string, DepthEstimationPipeline> DepthEstimationPipelines { get; } = new Dictionary<string, DepthEstimationPipeline>();
        /// <summary>
        /// The default depth estimation model. Used if no model is specified.
        /// </summary>
        public string DefaultDepthEstimationModel { get; set; } = "onnx-community/depth-anything-v2-small";
        /// <summary>
        /// Result from a WebGPU support check
        /// </summary>
        public bool WebGPUSupported { get; private set; }
        /// <summary>
        /// Cache for generated 2DZ images keyed by the image source string
        /// </summary>
        public Dictionary<string, HTMLImageElement> Cached2DZImages { get; } = new Dictionary<string, HTMLImageElement>();
        ActionCallback<ModelLoadProgress> OnProgress => new ActionCallback<ModelLoadProgress>(Pipeline_OnProgress);
        BlazorJSRuntime JS;
        Pipelines? Pipelines = null;
        public DepthEstimationService(BlazorJSRuntime js)
        {
            JS = js;
            WebGPUSupported = !JS.IsUndefined("navigator.gpu?.requestAdapter");
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
        public async Task<DepthEstimationPipeline> GetDepthEstimationPipeline(string? model = null, bool useWebGPU = true)
        {
            if (string.IsNullOrEmpty(model))
            {
                model = DefaultDepthEstimationModel;
            }
            if (!WebGPUSupported)
            {
                useWebGPU = false;
            }
            var key = useWebGPU ? $"{model}+webgpu" : model;
            if (DepthEstimationPipelines.TryGetValue(key, out var depthEstimationPipeline))
            {
                return depthEstimationPipeline;
            }
            await LoadLimiter.WaitAsync();
            try
            {
                Loading = true;
                if (Pipelines == null)
                {
                    Log($"Initializing... ", false);
                    Pipelines = await Pipelines.Init();
                    Log($"Done");
                }
                Log($"Depth Estimation Pipeline with WebGPU loading... ", false);
                depthEstimationPipeline = await Pipelines.DepthEstimationPipeline(model, new PipelineOptions
                {
                    Device = useWebGPU ? "webgpu" : null,
                    OnProgress = OnProgress,
                });
                DepthEstimationPipelines[key] = depthEstimationPipeline;
                Log($"Done");
                return depthEstimationPipeline;
            }
            finally
            {
                Loading = false;
                ModelProgresses.Clear();
                StateHasChanged();
                LoadLimiter.Release();
            }
        }
        public async Task<HTMLImageElement> ImageTo2DZImage(HTMLImageElement image, string? model = null, bool useWebGPU = true)
        {
            var source = image.Src;
            if (!Cached2DZImages.TryGetValue(source, out var imageWithDepth))
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
                Cached2DZImages[source] = imageWithDepth;
            }
            return imageWithDepth;
        }
        public async Task<HTMLImageElement> ImageTo2DZImage(string source, string? model = null, bool useWebGPU = true)
        {
            if (!Cached2DZImages.TryGetValue(source, out var imageWithDepth))
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
                Cached2DZImages[source] = imageWithDepth;
            }
            return imageWithDepth;
        }
        async Task<string> Create2DZObjectUrl(HTMLImageElement rgbImage, Uint8Array grayscale1BPPUint8Array, int width, int height)
        {
            var outWidth = width * 2;
            var outHeight = height;
            var grayscale1BPPBytes = grayscale1BPPUint8Array.ReadBytes();
            var depthmapRGBABytes = Grayscale1BPPToRGBA(grayscale1BPPBytes, width, height);
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
        async Task<string> CreateDepthImageObjectUrl(Uint8Array grayscale1BPPUint8Array, int width, int height)
        {
            var grayscale1BPPBytes = grayscale1BPPUint8Array.ReadBytes();
            var depthmapRGBABytes = Grayscale1BPPToRGBA(grayscale1BPPBytes, width, height);
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
