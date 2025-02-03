using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// https://github.com/huggingface/transformers/blob/62db3e6ed67a74cc1ed1436acd9973915c0a4475/src/transformers/models/vitpose/modeling_vitpose.py#L43
    /// </summary>
    public class VitPoseEstimatorOutput : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public VitPoseEstimatorOutput(IJSInProcessObjectReference _ref) : base(_ref) { }

        public (float x, float y, float width, float height) BBox => JSRef!.Get<(float x, float y, float width, float height)>("bbox");
        public List<(float x, float y)> Keypoints => JSRef!.Get<List<(float x, float y)>>("keypoints");
        public List<int> Labels => JSRef!.Get<List<int>>("labels");
        public List<float> Scores => JSRef!.Get<List<float>>("scores");
    }
}
