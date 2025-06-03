using Microsoft.JSInterop;

namespace SpawnDev.BlazorJS.TransformersJS
{
    public class Size : JSObject
    {
        /// <inheritdoc/>
        public Size(IJSInProcessObjectReference _ref) : base(_ref) { }

        public Size() : base(JS.New("Object"))
        {
            Width = 0;
            Height = 0;
        }
        public Size(double width, double height) : base(JS.New("Object"))
        {
            Width = width;
            Height = height;
        }

        public double Width
        {
            get => JSRef!.Get<double>("width");
            set => JSRef!.Set("width", value);
        }

        public double Height
        {
            get => JSRef!.Get<double>("height");
            set => JSRef!.Set("height", value);
        }
    }
}
