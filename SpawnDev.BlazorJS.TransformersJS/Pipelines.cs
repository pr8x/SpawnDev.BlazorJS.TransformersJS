using Microsoft.JSInterop;
using SpawnDev.BlazorJS.IJSInProcessObjectReferenceAnyKey;
using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class Pipelines : JSObject
    {
        static BlazorJSRuntime JS => BlazorJSRuntime.JS;
        /// <summary>
        /// Transformers.js bundled with this library<br/>
        /// Downloaded from:<br/>
        /// https://cdn.jsdelivr.net/npm/@huggingface/transformers
        /// </summary>
        public static string LatestBundledVersionSrc { get; } = $"./_content/SpawnDev.BlazorJS.TransformersJS/transformers.js";
        /// <summary>
        /// Transformers.js CDN URL<br/>
        /// https://cdn.jsdelivr.net/npm/@huggingface/transformers
        /// </summary>
        public static string LatestCDNVersionSrc { get; } = $"https://cdn.jsdelivr.net/npm/@huggingface/transformers";
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public Pipelines(IJSInProcessObjectReference _ref) : base(_ref) { }
        public static bool IsInit => !JS.IsUndefined("Pipelines");
        public static async Task<Pipelines> Init(string? srcUrl = null)
        {
            // var transformersModule = await import('https://cdn.jsdelivr.net/npm/@huggingface/transformers')
            srcUrl = srcUrl ?? LatestBundledVersionSrc;
            var pipelines = JS.Get<Pipelines>("Pipelines");
            if (pipelines != null) return pipelines;
            var module = await JS.Import(LatestBundledVersionSrc);
            if (module == null) throw new Exception("WebTorrentService could not be initialized.");
            //var WebTorrentClass = module.GetExport<Function>("default");
            // set WebTorrent on the global scope so it can be used globally
            JS.Set("Pipelines", module);
            pipelines = JS.Get<Pipelines>("Pipelines");
            return pipelines;
        }
        //The task defining which pipeline will be returned.Currently accepted tasks are:
        //"audio-classification": will return a AudioClassificationPipeline.
        //"automatic-speech-recognition": will return a AutomaticSpeechRecognitionPipeline.
        //"depth-estimation": will return a DepthEstimationPipeline.
        //"document-question-answering": will return a DocumentQuestionAnsweringPipeline.
        //"feature-extraction": will return a FeatureExtractionPipeline.
        //"fill-mask": will return a FillMaskPipeline.
        //"image-classification": will return a ImageClassificationPipeline.
        //"image-segmentation": will return a ImageSegmentationPipeline.
        //"image-to-text": will return a ImageToTextPipeline.
        //"object-detection": will return a ObjectDetectionPipeline.
        //"question-answering": will return a QuestionAnsweringPipeline.
        //"summarization": will return a SummarizationPipeline.
        //"text2text-generation": will return a Text2TextGenerationPipeline.
        //"text-classification" (alias "sentiment-analysis" available): will return a TextClassificationPipeline.
        //"text-generation": will return a TextGenerationPipeline.
        //"token-classification" (alias "ner" available): will return a TokenClassificationPipeline.
        //"translation": will return a TranslationPipeline.
        //"translation_xx_to_yy": will return a TranslationPipeline.
        //"zero-shot-classification": will return a ZeroShotClassificationPipeline.
        //"zero-shot-audio-classification": will return a ZeroShotAudioClassificationPipeline.
        //"zero-shot-image-classification": will return a ZeroShotImageClassificationPipeline.
        //"zero-shot-object-detection": will return a ZeroShotObjectDetectionPipeline.
        public static Task<TPipeline> Pipeline<TPipeline>(string type, string? source = null, PipelineOptions? pipelineOptions = null) where TPipeline : Pipeline 
            => pipelineOptions == null ? JS.CallAsync<TPipeline>("Pipelines.pipeline", type, source) : JS.CallAsync<TPipeline>("Pipelines.pipeline", type, source, pipelineOptions);

        public Task<DepthEstimationPipeline> DepthEstimationPipeline(string? source = null, PipelineOptions? pipelineOptions = null) => Pipeline<DepthEstimationPipeline>("depth-estimation", source, pipelineOptions);
    }
}
