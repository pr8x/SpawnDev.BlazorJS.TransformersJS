using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class PipelineOptions
    {
        /// <summary>
        /// Possible options:<br/>
        /// "webgpu"
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Device { get; set; }
    }
}
