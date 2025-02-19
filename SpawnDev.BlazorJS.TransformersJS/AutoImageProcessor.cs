using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class AutoImageProcessor : Pipeline
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public AutoImageProcessor(IJSInProcessObjectReference _ref) : base(_ref) { }
        public static Task<AutoImageProcessor> FromPretrained(string modelId, FromPretrainedOptions? options = null) => JS.CallAsync<AutoImageProcessor>("Transformers.AutoImageProcessor.from_pretrained", modelId, options);
        public static Task<AutoImageProcessor> FromPretrained(string modelId, PipelineOptions? options = null) => JS.CallAsync<AutoImageProcessor>("Transformers.AutoImageProcessor.from_pretrained", modelId, options);
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
    }
}
