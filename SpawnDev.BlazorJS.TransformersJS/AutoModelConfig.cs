using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class AutoModelConfig : JSObject
    {
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="_ref"></param>
        public AutoModelConfig(IJSInProcessObjectReference _ref) : base(_ref) { }

        public List<string> Architectures => JSRef!.Get<List<string>>("architectures");

        public List<(int, int)> Edges => JSRef!.Get<List<(int, int)>>("edges");

        public Dictionary<int, string> Id2Label => JSRef!.Get<Dictionary<int, string>>("id2label");

        public (int height, int width) ImageSize => JSRef!.Get<(int height, int width)>("image_size");

        public float InitializerRange => JSRef!.Get<float>("initializer_range");

        public bool IsEncoderDecoder => JSRef!.Get<bool>("is_encoder_decoder");

        public Dictionary<string, int> Label2Id => JSRef!.Get<Dictionary<string, int>>("label2id");

        public string ModelType => JSRef!.Get<string>("model_type");

        public (int height, int width) PatchSize => JSRef!.Get<(int height, int width)>("patch_size");

        public float ScaleFactor => JSRef!.Get<float>("scale_factor");

        public string TransformersVersion => JSRef!.Get<string>("transformers_version");
    }
}
