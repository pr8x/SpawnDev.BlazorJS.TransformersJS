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
        /// <summary>
        /// Runs the pipeline _call method asynchronously
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task<T> _Call<T>(params object[] args) => JSRef!.CallAsync<T>("_call.apply", JSRef, args);
        /// <summary>
        /// Runs the pipeline _call method synchronously
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public T _CallSync<T>(params object[] args) => JSRef!.Call<T>("_call.apply", JSRef, args);
    }
}
