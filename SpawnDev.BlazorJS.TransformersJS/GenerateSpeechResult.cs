using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;
using SpawnDev.BlazorJS.TransformersJS.ONNX;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class GenerateSpeechResult : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public GenerateSpeechResult(IJSInProcessObjectReference _ref) : base(_ref) { }
        public Tensor<Float32Array> Spectrogram => JSRef!.Get<Tensor<Float32Array>>("spectrogram");
        public Tensor<Float32Array> Waveform => JSRef!.Get<Tensor<Float32Array>>("waveform");
    }
}
