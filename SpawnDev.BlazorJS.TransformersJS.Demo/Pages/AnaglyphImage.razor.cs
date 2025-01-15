using Microsoft.AspNetCore.Components;
using SpawnDev.BlazorJS.JSObjects;
using SpawnDev.BlazorJS.TransformersJS.Demo.Renderers;
using SpawnDev.BlazorJS.TransformersJS.Demo.Services;

namespace SpawnDev.BlazorJS.TransformersJS.Demo.Pages
{
    public partial class AnaglyphImage
    {
        ElementReference canvasElRef;
        HTMLCanvasElement? canvas = null;

        [Inject]
        DepthEstimationService DepthEstimationService { get; set; }

        [Inject]
        BlazorJSRuntime JS { get; set; }

        [Parameter]
        public string DepthEstimationModel { get; set; } = "";

        [Parameter]
        public bool UseWebGPU { get; set; } = true;

        [Parameter]
        public string Source { get; set; } = "";

        [Parameter]
        public string Style { get; set; } = "";

        [Parameter]
        public int AnaglyphProfile { get; set; } 

        [Parameter]
        public float Focus3D { get; set; } = 0.5f;

        [Parameter]
        public float Level3D { get; set; } = 1.0f;

        public string GeneratedSource => string.IsNullOrEmpty(_GeneratedSource) ? Source : _GeneratedSource;

        AnaglyphRenderer? anaglyphRenderer { get; set; }

        string _GeneratedSource = "";
        bool IsDirty = false;

        protected override void OnParametersSet()
        {
            Console.WriteLine($"OnParametersSet {IsDirty}");
            IsDirty = true;
        }
        public async Task<bool> DownloadImage(string filename, float? quality = null)
        {
            if (anaglyphRenderer == null) return false;
            var ext = filename.Split(".").Last().ToLowerInvariant();
            string? mimeType = null;
            switch (ext)
            {
                case "jpg":
                case "jpeg":
                    mimeType = "image/jpeg";
                    break;
                default:
                    mimeType = $"image/{ext}";
                    break;
            }
            var objectUrl = await anaglyphRenderer.ToObjectUrl(mimeType, quality);
            if (string.IsNullOrEmpty(objectUrl)) return false;
            DownloadFile(objectUrl, filename);
            URL.RevokeObjectURL(objectUrl);
            return true;
        }
        void DownloadFile(string url, string filename)
        {
            using var document = JS.GetDocument();
            using var a = document!.CreateElement<HTMLAnchorElement>("a");
            a.Href = url;
            a.Download = filename;
            document.Body!.AppendChild(a);
            a.Click();
            document.Body.RemoveChild(a);
        }
        bool beenRendered = false;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Console.WriteLine($"OnAfterRenderAsync: IsDirty {IsDirty}");
            canvas ??= new HTMLCanvasElement(canvasElRef);
            if (IsDirty)
            {
                IsDirty = false;
                anaglyphRenderer ??= new AnaglyphRenderer(canvas);
                try
                {
                    if (!beenRendered)
                    {
                        beenRendered = true;
                        using var image = await HTMLImageElement.CreateFromImageAsync(Source);
                        anaglyphRenderer.SetInput(image, "2d");
                        anaglyphRenderer.Render();
                    }
                    var imageWithDepth = await DepthEstimationService.ImageTo2DZImage(Source, DepthEstimationModel, UseWebGPU);
                    anaglyphRenderer.Level3D = Level3D;
                    anaglyphRenderer.Focus3D = Focus3D;
                    anaglyphRenderer.ProfileIndex = AnaglyphProfile;
                    anaglyphRenderer.SetInput(imageWithDepth, "2dz");
                    anaglyphRenderer.Render();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ImageToAnaglyph error: {ex.Message}");
                }
                StateHasChanged();
            }
        }
    }
}
