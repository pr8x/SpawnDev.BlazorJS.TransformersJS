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
        /// file: "onnx/model.onnx",
        /// loaded: 99060839,
        /// name: "onnx-community/depth-anything-v2-small",
        /// progress: 100,
        /// status: "progress",
        /// total: 99060839
        /// 
        /// file: "onnx/model.onnx",
        /// name: "onnx-community/depth-anything-v2-small",
        /// status: "done"
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("progress_callback")]
        public ActionCallback<ModelLoadProgress>? OnProgress { get; set; }
    }
    public class FromPretrainedSubOptions
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("prepare_inputs_embeds")]
        public string? PrepareInputsEmbeds { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("language_model")]
        public string? LanguageModel { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("lm_head")]
        public string? LmHead { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("gen_head")]
        public string? GenHead { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("gen_img_embeds")]
        public string? GenImgEmbeds { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("image_decode")]
        public string? ImageDecode { get; set; }
    }
    public class FromPretrainedOptions
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("device")]
        public FromPretrainedSubOptions? Device { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("dtype")]
        public FromPretrainedSubOptions? Dtype { get; set; }

        /// <summary>
        /// file: "onnx/model.onnx",
        /// loaded: 99060839,
        /// name: "onnx-community/depth-anything-v2-small",
        /// progress: 100,
        /// status: "progress",
        /// total: 99060839
        /// 
        /// file: "onnx/model.onnx",
        /// name: "onnx-community/depth-anything-v2-small",
        /// status: "done"
        /// </summary>
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        //[JsonPropertyName("onDownloadProgress")]
        //public ActionCallback<ModelLoadProgress>? OnProgress { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("progress_callback")]
        public ActionCallback<ModelLoadProgress>? OnProgress { get; set; }
    }
}
