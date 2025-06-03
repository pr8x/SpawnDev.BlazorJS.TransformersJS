using Microsoft.JSInterop;
using SpawnDev.BlazorJS.IJSInProcessObjectReferenceAnyKey;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// 
    /// https://github.com/huggingface/transformers.js/blob/main/src/models/auto/image_processing_auto.js
    /// </summary>
    public class AutoImageProcessor : Pipeline
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public AutoImageProcessor(IJSInProcessObjectReference _ref) : base(_ref) { }
        public static Task<AutoImageProcessor> FromPretrained(string modelId) => JS.CallAsync<AutoImageProcessor>("Transformers.AutoImageProcessor.from_pretrained", modelId);
        public static Task<AutoImageProcessor> FromPretrained(string modelId, FromPretrainedOptions? options) => JS.CallAsync<AutoImageProcessor>("Transformers.AutoImageProcessor.from_pretrained", modelId, options);
        public static Task<AutoImageProcessor> FromPretrained(string modelId, PipelineOptions? options) => JS.CallAsync<AutoImageProcessor>("Transformers.AutoImageProcessor.from_pretrained", modelId, options);
        public Task<AutoImageProcessorResult> Call(RawImage source) => _Call<AutoImageProcessorResult>(source);
        /// <summary>
        /// Transform the heatmaps into keypoint predictions and transform them back to the image.
        /// </summary>
        /// <param name="heatmaps"></param>
        /// <param name="boxes">List or array of bounding boxes for each image. Each box should be a list of 4 floats representing the bounding box coordinates in COCO format(top_left_x, top_left_y, width, height).</param>
        /// <returns>VitPoseEstimatorOutput</returns>
        public VitPoseEstimatorOutput[][] PostProcessPoseEstimation(TensorProxy<Float32Array> heatmaps, (float, float, float, float)[][] boxes) => JSRef!.Call<VitPoseEstimatorOutput[][]>("post_process_pose_estimation", heatmaps, boxes);
        /// <summary>
        /// Transform the heatmaps into keypoint predictions and transform them back to the image.
        /// </summary>
        /// <param name="heatmaps"></param>
        /// <param name="boxes">List or array of bounding boxes for each image. Each box should be a list of 4 floats representing the bounding box coordinates in COCO format(top_left_x, top_left_y, width, height).</param>
        /// <returns>VitPoseEstimatorOutput</returns>
        public VitPoseEstimatorOutput[][] PostProcessPoseEstimation(TensorProxy<Float32Array> heatmaps, (int, int, int, int)[][] boxes) => JSRef!.Call<VitPoseEstimatorOutput[][]>("post_process_pose_estimation", heatmaps, boxes);
        public Size Size
        {
            get => JSRef!.Get<Size>("size");
            set => JSRef!.Set("size", value);
        }
        public ImageProcessorConfiguration Config => JSRef!.Get<ImageProcessorConfiguration>("config");
    }
    public class ImageProcessorConfiguration : JSObject
    {
        /// <inheritdoc/>
        public ImageProcessorConfiguration(IJSInProcessObjectReference _ref) : base(_ref) { }
        public bool DoAffineTransform { get => JSRef!.Get<bool>("do_affine_transform"); set => JSRef!.Set("do_affine_transform", value); }
        public bool DoNormalize { get => JSRef!.Get<bool>("do_normalize"); set => JSRef!.Set("do_normalize", value); }
        public bool DoRescale { get => JSRef!.Get<bool>("do_rescale"); set => JSRef!.Set("do_rescale", value); }
        public float[] ImageMean
        {
            get => JSRef!.Get<float[]>("image_mean"); 
            set => JSRef!.Set("image_mean", value);
        }
        public string ImageProcessorType
        {
            get => JSRef!.Get<string>("image_processor_type"); 
            set => JSRef!.Set("image_processor_type", value);
        }
        public float[] ImageStd
        {
            get => JSRef!.Get<float[]>("image_std"); 
            set => JSRef!.Set("image_std", value);
        }
        public float NormalizeFactor
        {
            get => JSRef!.Get<float>("normalize_factor"); 
            set => JSRef!.Set("normalize_factor", value);
        }
        public double RescaleFactor
        {
            get => JSRef!.Get<double>("rescale_factor"); 
            set => JSRef!.Set("rescale_factor", value);
        }
        public Size Size
        {
            get => JSRef!.Get<Size>("size");
            set => JSRef!.Set("size", value);
        }
    }
}
