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
        MultiViewImageService MultiViewImageService { get; set; }

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
                    var imageWithDepth = await MultiViewImageService.ImageTo2DZImage(Source, DepthEstimationModel, UseWebGPU);
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
