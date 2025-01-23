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

        public static Task<MultiModalityCausalLM> FromPretrained(string modelId, FromPretrainedOptions? options = null) => JS.CallAsync<MultiModalityCausalLM>("Transformers.MultiModalityCausalLM.from_pretrained", modelId, options);
        public static Task<MultiModalityCausalLM> FromPretrained(string modelId, PipelineOptions? options = null) => JS.CallAsync<MultiModalityCausalLM>("Transformers.MultiModalityCausalLM.from_pretrained", modelId, options);
    }

}
