using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class AutoTokenizerResult : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public AutoTokenizerResult(IJSInProcessObjectReference _ref) : base(_ref) { }
        public TensorProxy<BigInt64Array> InputIds => JSRef!.Get<TensorProxy<BigInt64Array>>("input_ids");
        public TensorProxy<BigInt64Array> AttentionMask => JSRef!.Get<TensorProxy<BigInt64Array>>("attention_mask");
    }
}
