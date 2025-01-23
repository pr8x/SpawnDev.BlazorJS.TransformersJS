using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class SpeechT5HifiGan : Pipeline
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public SpeechT5HifiGan(IJSInProcessObjectReference _ref) : base(_ref) { }
        public static Task<SpeechT5HifiGan> FromPretrained(string modelId, FromPretrainedOptions? options = null) => JS.CallAsync<SpeechT5HifiGan>("Transformers.SpeechT5HifiGan.from_pretrained", modelId, options);
        public static Task<SpeechT5HifiGan> FromPretrained(string modelId, PipelineOptions? options = null) => JS.CallAsync<SpeechT5HifiGan>("Transformers.SpeechT5HifiGan.from_pretrained", modelId, options);
    }
}
