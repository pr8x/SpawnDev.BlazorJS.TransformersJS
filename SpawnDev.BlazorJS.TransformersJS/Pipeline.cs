using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// The Pipeline class is the class from which all pipelines inherit. Refer to this class for methods shared across different pipelines.<br/>
    /// https://huggingface.co/docs/transformers.js/api/pipelines#module_pipelines.Pipeline
    /// </summary>
    public class Pipeline : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public Pipeline(IJSInProcessObjectReference _ref) : base(_ref) { }
        /// <summary>
        /// Calls JS object's dispose() method
        /// </summary>
        public void DisposeJS() => JSRef!.CallVoid("dispose");

        public Task<JSObject> _Call(params object[] args) => _Call<JSObject>(args);
        public Task<T> _Call<T>(params object[] args) => JSRef!.CallAsync<T>("_call.apply", JSRef, args);
    }
}
