using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class AutoProcessor : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public AutoProcessor(IJSInProcessObjectReference _ref) : base(_ref) { }
        public static Task<AutoProcessor> FromPretrained(string modelId, FromPretrainedOptions? options = null) => JS.CallAsync<AutoProcessor>("Transformers.AutoProcessor.from_pretrained", modelId, options);
        public static Task<AutoProcessor> FromPretrained(string modelId, PipelineOptions? options = null) => JS.CallAsync<AutoProcessor>("Transformers.AutoProcessor.from_pretrained", modelId, options);
    }

}
