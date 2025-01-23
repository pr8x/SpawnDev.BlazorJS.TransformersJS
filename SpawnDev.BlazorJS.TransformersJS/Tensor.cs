using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;
using System.Xml.Linq;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class Tensor<TData> : Tensor
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public Tensor(IJSInProcessObjectReference _ref) : base(_ref) { }

        public Tensor(string type, TData data, IEnumerable<int> dims) : base(JS.New("Transformers.Tensor", type, data, dims)) { }
        /// <summary>
        /// The tensor data as type TData
        /// </summary>
        public TData Data => JSRef!.Get<TData>("data");
    }
    public class Tensor : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public Tensor(IJSInProcessObjectReference _ref) : base(_ref) { }

        public Tensor(string type, TypedArray data, IEnumerable<int> dims) : base(JS.New("Transformers.Tensor", type, data, dims)) { }

        public int[] Dims => JSRef!.Get<int[]>("dims");

        public string Type => JSRef!.Get<string>("type");

        public string Location => JSRef!.Get<string>("location");

        public int Size => JSRef!.Get<int>("size");
        /// <summary>
        /// The tensor data as type TData
        /// </summary>
        public TData Get_Data<TData>() => JSRef!.Get<TData>("data");
    }
}
