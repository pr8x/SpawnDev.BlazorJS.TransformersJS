using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class AutoModelResult : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public AutoModelResult(IJSInProcessObjectReference _ref) : base(_ref) { }

        public TensorProxy<float> Heatmaps => JSRef!.Get<TensorProxy<float>>("heatmaps");
    }
}
