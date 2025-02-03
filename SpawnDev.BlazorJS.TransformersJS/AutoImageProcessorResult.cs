using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class AutoImageProcessorResult : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public AutoImageProcessorResult(IJSInProcessObjectReference _ref) : base(_ref) { }
        public List<(int height, int width)> OriginalSizes => JSRef!.Get<List<(int height, int width)>>("original_sizes");
        public TensorProxy<float> PixelValues => JSRef!.Get<TensorProxy<float>>("pixel_values");
        public List<(int height, int width)> ReshapedInputSizes => JSRef!.Get<List<(int height, int width)>>("reshaped_input_sizes");
    }
}
