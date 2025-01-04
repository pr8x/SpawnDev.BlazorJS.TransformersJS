using Microsoft.AspNetCore.Components;
using SpawnDev.BlazorJS.JSObjects;
using File = SpawnDev.BlazorJS.JSObjects.File;

namespace SpawnDev.BlazorJS.TransformersJS.Demo.Pages
{
    public partial class DepthFrom2DTest
    {
        [Inject]
        BlazorJSRuntime JS { get; set; } = default!;

        bool beenInit = false;
        bool busy = true;
        string logMessage = "";
        ElementReference fileInputRef;
        HTMLInputElement? fileInput;
        string outputFileName = "depthmap.png";
        Pipelines? Pipelines = null;
        DepthEstimationPipeline? DepthEstimationPipeline = null;
        string? fileObjectUrl = null;
        string? depthObjectUrl = null;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!beenInit && fileInput == null)
            {
                fileInput = new HTMLInputElement(fileInputRef);
                fileInput.OnChange += FileInput_OnChange;
                Log($"Pipelines initializing... ", false);
                Pipelines = await Pipelines.Init();
                Log($"Done");
                busy = false;
                StateHasChanged();
            }
        }
        async Task LoadDepthEstimationPipeline()
        {
            if (Pipelines == null) return;
            busy = true;
            try
            {
                Log($"Depth Estimation Pipeline loading... ", false);
                DepthEstimationPipeline = await Pipelines.DepthEstimationPipeline();
                Log($"Done");
            }
            catch
            {
                Log($"Error");
            }
            beenInit = true;
            busy = false;
            StateHasChanged();
        }
        async Task LoadDepthEstimationPipelineWebGPU()
        {
            if (Pipelines == null) return;
            busy = true;
            try
            {
                Log($"Depth Estimation Pipeline with WebGPU loading... ", false);
                DepthEstimationPipeline = await Pipelines.DepthEstimationPipeline(pipelineOptions: new PipelineOptions { Device = "webgpu" });
                Log($"Done");
            }
            catch
            {
                Log($"Error");
            }
            beenInit = true;
            busy = false;
            StateHasChanged();
        }
        async void FileInput_OnChange(Event ev)
        {
            using var files = fileInput!.Files;
            using var file = files?.FirstOrDefault();
            if (file == null) return;
            try
            {
                await ProcessFile(file);
            }
            catch { }
        }
        void Log(string msg, bool newLine = true)
        {
            if (newLine)
            {
                logMessage += $"{msg}<br/>";
            }
            else
            {
                logMessage += $"{msg}";
            }
            StateHasChanged();
        }
        string CreateDepthImageDataUrl(Uint8Array grayscaleData, int width, int height)
        {
            var grayscaleDataBytes = grayscaleData.ReadBytes();
            var rgbaBytes = GrayscaleToRGBA(grayscaleDataBytes, width, height);
            using var canvas = new HTMLCanvasElement(width, height);
            using var ctx = canvas.Get2DContext();
            ctx.PutImageBytes(rgbaBytes, width, height);
            return canvas.ToDataURL("image/png");
        }
        byte[] GrayscaleToRGBA(byte[] grayscaleData, int width, int height)
        {
            var ret = new byte[width * height * 4];
            for (var i = 0; i < grayscaleData.Length; i++)
            {
                var grayValue = grayscaleData[i];
                ret[i * 4] = grayValue;     // Red
                ret[i * 4 + 1] = grayValue; // Green
                ret[i * 4 + 2] = grayValue; // Blue
                ret[i * 4 + 3] = 255;       // Alpha
            }
            return ret;
        }
        async Task ProcessFile(File file)
        {
            busy = true;
            StateHasChanged();
            if (!string.IsNullOrEmpty(fileObjectUrl))
            {
                URL.RevokeObjectURL(fileObjectUrl);
                fileObjectUrl = null;
            }
            if (!string.IsNullOrEmpty(depthObjectUrl))
            {
                URL.RevokeObjectURL(depthObjectUrl);
                depthObjectUrl = null;
            }
            Log("Creating image data url... ", false);
            fileObjectUrl = await FileReader.ReadAsDataURLAsync(file);
            Log("Done");
            StateHasChanged();
            try
            {
                Log("Estimating depth... ", false);
                using var depthResult = await DepthEstimationPipeline!.Call(fileObjectUrl!);
                using var depthInfo = depthResult.Depth;
                using var depthMapData = depthInfo.Data;
                depthObjectUrl = CreateDepthImageDataUrl(depthMapData, depthInfo.Width, depthInfo.Height);
                Log("Done");
            }
            catch (Exception ex)
            {
                Log($"Error");
            }
            busy = false;
            StateHasChanged();
        }
        public void Dispose()
        {
            if (fileInput != null)
            {
                fileInput.OnChange -= FileInput_OnChange;
                fileInput.Dispose();
            }
            if (!string.IsNullOrEmpty(fileObjectUrl))
            {
                URL.RevokeObjectURL(fileObjectUrl);
                fileObjectUrl = null;
            }
            if (!string.IsNullOrEmpty(depthObjectUrl))
            {
                URL.RevokeObjectURL(depthObjectUrl);
                depthObjectUrl = null;
            }
            beenInit = false;
        }
        async Task<(int width, int height, Uint8ClampedArray data)> LoadImage(File file)
        {
            // get RGB data from image file
            var imageDataUrl = await FileReader.ReadAsDataURLAsync(file);
            using var image = await HTMLImageElement.CreateFromImageAsync(imageDataUrl!);
            using var canvas = new HTMLCanvasElement();
            using var ctx = canvas.Get2DContext();
            var width = image.Width;
            var height = image.Height;
            canvas.Width = width;
            canvas.Height = height;
            ctx.DrawImage(image, 0, 0);
            var imageData = ctx.GetImageData(0, 0, width, height);
            var data = imageData.Data;
            return (width, height, data);
        }
        async Task<(int width, int height, byte[] data)> LoadImageBytes(File file)
        {
            var ret = await LoadImage(file);
            var width = ret.width;
            var height = ret.height;
            using var uint8ClampedArray = ret.data;
            var data = uint8ClampedArray.ReadBytes();
            return (width, height, data);
        }
        public static float[][,] ToRGB(byte[] data, int width, int height, bool alpha = false, int stride = 0)
        {
            stride = stride == 0 ? width : stride;
            float[,] r = new float[height, width];
            float[,] g = new float[height, width];
            float[,] b = new float[height, width];
            if (alpha)
            {
                float[,] a = new float[height, width];
                //Parallel.For(0, height, delegate (int y)
                for (int y = 0; y < height; y++)
                {
                    int num3 = y * stride;
                    for (int x = 0; x < width; x++)
                    {
                        int num4 = num3 + x * 4;
                        r[y, x] = (float)(int)data[num4] / 255f;
                        g[y, x] = (float)(int)data[num4 + 1] / 255f;
                        b[y, x] = (float)(int)data[num4 + 2] / 255f;
                        a[y, x] = (float)(int)data[num4 + 3] / 255f;
                    }
                };
                return new float[4][,] { r, g, b, a };
            }
            //Parallel.For(0, height, delegate (int y)
            for (int y = 0; y < height; y++)
            {
                int num = y * stride;
                for (int x = 0; x < width; x++)
                {
                    int num2 = num + x * 4;
                    r[y, x] = (float)(int)data[num2] / 255f;
                    g[y, x] = (float)(int)data[num2 + 1] / 255f;
                    b[y, x] = (float)(int)data[num2 + 2] / 255f;
                }
            };
            return new float[3][,] { r, g, b };
        }
    }
}
