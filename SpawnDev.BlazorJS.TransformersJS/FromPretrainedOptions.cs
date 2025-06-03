using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class FromPretrainedOptions
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("device")]
        public Union<FromPretrainedSubOptions, string>? Device { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("dtype")]
        public Union<FromPretrainedSubOptions, string>? Dtype { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("progress_callback")]
        public ActionCallback<ModelLoadProgress>? OnProgress { get; set; }
    }
}
