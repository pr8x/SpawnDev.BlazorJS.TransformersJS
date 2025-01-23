using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class AutoTokenizer : Pipeline
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public AutoTokenizer(IJSInProcessObjectReference _ref) : base(_ref) { }
        public static Task<AutoTokenizer> FromPretrained(string modelId, FromPretrainedOptions? options = null) => JS.CallAsync<AutoTokenizer>("Transformers.AutoTokenizer.from_pretrained", modelId, options);
        public static Task<AutoTokenizer> FromPretrained(string modelId, PipelineOptions? options = null) => JS.CallAsync<AutoTokenizer>("Transformers.AutoTokenizer.from_pretrained", modelId, options);
        public AutoTokenizerResult Call(string source) => _CallSync<AutoTokenizerResult>(source);
    }
}
