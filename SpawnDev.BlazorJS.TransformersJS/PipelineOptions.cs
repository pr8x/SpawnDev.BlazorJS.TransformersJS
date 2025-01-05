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


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ActionCallback<JSObject>? OnProgress { get; set; }
    }
}
