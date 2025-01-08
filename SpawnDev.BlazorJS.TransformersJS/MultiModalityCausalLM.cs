using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class MultiModalityCausalLM : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public MultiModalityCausalLM(IJSInProcessObjectReference _ref) : base(_ref) { }

        public static Task<MultiModalityCausalLM> FromPretrained(string modelId, FromPretrainedOptions? options = null)
        {
            var ret = JS.CallAsync<MultiModalityCausalLM>("Pipelines.MultiModalityCausalLM.from_pretrained", modelId, options);
            return ret;
        }
        public static Task<MultiModalityCausalLM> FromPretrained(string modelId, PipelineOptions? options = null)
        {
            var ret = JS.CallAsync<MultiModalityCausalLM>("Pipelines.MultiModalityCausalLM.from_pretrained", modelId, options);
            return ret;
        }
    }

}
