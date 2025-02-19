using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// Describes normalization parameters when preprocessing the image as model input.<br/>
    /// Data element are ranged from 0 to 255.<br/>
    /// https://onnxruntime.ai/docs/api/js/interfaces/TensorToDataUrlOptions.html#norm (Type declaration)
    /// </summary>
    public class NormalizationParameters
    {
        /// <summary>
        /// The 'bias' value for image normalization.<br/>
        /// - If omitted, use default value 0.<br/>
        /// - If it's a single number, apply to each channel<br/>
        /// - If it's an array of 3 or 4 numbers, apply element-wise. Number of elements need to match the number of channels for the corresponding image format
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Union<float, float[]>? Bias { get; set; }
        /// <summary>
        /// The 'mean' value for image normalization.<br/>
        /// - If omitted, use default value 255.<br/>
        /// - If it's a single number, apply to each channel<br/>
        /// - If it's an array of 3 or 4 numbers, apply element-wise. Number of elements need to match the number of channels for the corresponding image format
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Union<float, float[]>? Mean { get; set; }
    }
}
