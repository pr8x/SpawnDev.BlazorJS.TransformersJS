using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// Tensor options for ToImageData()
    /// </summary>
    public class TensorToImageDataOptions
    {
        /// <summary>
        /// Default "RGB"
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Format { get; set; }
        /// <summary>
        /// Default "NCHW"
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TensorLayout { get; set; }
    }
}
