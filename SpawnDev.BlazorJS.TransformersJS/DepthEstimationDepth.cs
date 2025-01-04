using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class DepthEstimationDepth : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public DepthEstimationDepth(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Depth map channel count
        /// </summary>
        public int Channels => JSRef!.Get<int>("channels");
        /// <summary>
        /// Depth map width
        /// </summary>
        public int Width => JSRef!.Get<int>("width");
        /// <summary>
        /// Depth map height
        /// </summary>
        public int Height => JSRef!.Get<int>("height");
        /// <summary>
        /// The depth map data
        /// </summary>
        public Uint8Array Data => JSRef!.Get<Uint8Array>("data");
    }
}
