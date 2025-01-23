using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class PipelineOptions
    {
        /// <summary>
        /// Possible options:<br/>
        /// "webgpu"<br/>
        /// "webgl"
        /// "webnn"
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Device { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("progress_callback")]
        public ActionCallback<ModelLoadProgress>? OnProgress { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("dtype")]
        public Union<FromPretrainedSubOptions, string>? Dtype { get; set; }
    }
}
