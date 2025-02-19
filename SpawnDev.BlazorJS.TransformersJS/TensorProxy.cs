using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// 
    /// </summary>
    public class TensorProxy : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public TensorProxy(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// 
        /// </summary>
        public virtual Tensor OrtTensor => JSRef!.Get<Tensor>("ort_tensor");
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public Tensor<TData> Get_OrtTensor<TData>() => JSRef!.Get<Tensor<TData>>("ort_tensor");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class TensorProxy<TData> : TensorProxy
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public TensorProxy(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// 
        /// </summary>
        public override Tensor<TData> OrtTensor => JSRef!.Get<Tensor<TData>>("ort_tensor");
    }
}
