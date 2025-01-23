using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class TensorProxy : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public TensorProxy(IJSInProcessObjectReference _ref) : base(_ref) { }
        public Tensor OrtTensor => JSRef!.Get<Tensor>("ort_tensor");
        public Tensor<T> Get_OrtTensor<T>() => JSRef!.Get<Tensor<T>>("ort_tensor");
    }
    public class TensorProxy<TTensorData> : TensorProxy
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public TensorProxy(IJSInProcessObjectReference _ref) : base(_ref) { }
        public new Tensor<TTensorData> OrtTensor => JSRef!.Get<Tensor<TTensorData>>("ort_tensor");
    }
}
