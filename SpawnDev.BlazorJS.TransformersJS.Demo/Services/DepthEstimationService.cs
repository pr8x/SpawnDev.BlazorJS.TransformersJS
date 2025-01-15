using System.Threading;

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
                return depthEstimationPipeline;
            }
            finally
            {
                LoadLimiter.Release();
            }
        }
    }
}
