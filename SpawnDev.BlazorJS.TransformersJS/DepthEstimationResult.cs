using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class DepthEstimationResult : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public DepthEstimationResult(IJSInProcessObjectReference _ref) : base(_ref) { }

        public DepthEstimationDepth Depth => JSRef!.Get<DepthEstimationDepth>("depth");

        public DepthEstimationPredictedDepth PredictedDepth => JSRef!.Get<DepthEstimationPredictedDepth>("predicted_depth");
    }
}
