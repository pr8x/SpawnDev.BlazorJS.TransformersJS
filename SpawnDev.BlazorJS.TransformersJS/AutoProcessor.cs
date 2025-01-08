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

        public static Task<AutoProcessor> FromPretrained(string modelId, FromPretrainedOptions? options = null)
        {
            var ret = JS.CallAsync<AutoProcessor>("Pipelines.AutoProcessor.from_pretrained", modelId, options);
            return ret;
        }
        public static Task<AutoProcessor> FromPretrained(string modelId, PipelineOptions? options = null)
        {
            var ret = JS.CallAsync<AutoProcessor>("Pipelines.AutoProcessor.from_pretrained", modelId, options);
            return ret;
        }
    }

}
