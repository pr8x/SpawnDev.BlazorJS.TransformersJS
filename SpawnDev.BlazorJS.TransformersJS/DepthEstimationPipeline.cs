using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// https://huggingface.co/docs/transformers.js/api/pipelines#pipelinesdepthestimationpipeline
    /// </summary>
    public class DepthEstimationPipeline : Pipeline
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public DepthEstimationPipeline(IJSInProcessObjectReference _ref) : base(_ref) { }

        public Task<DepthEstimationResult> Call(string source) => _Call<DepthEstimationResult>(source);
    }

}
