using Microsoft.AspNetCore.Components;
using SpawnDev.BlazorJS.JSObjects;
using SpawnDev.BlazorJS.WebWorkers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata;
using System;
using File = SpawnDev.BlazorJS.JSObjects.File;

namespace SpawnDev.BlazorJS.TransformersJS.Demo.Pages
{
    public partial class DepthFrom2DTest
    {
        [Inject]
        BlazorJSRuntime JS { get; set; } = default!;

        [Inject]
        WebWorkerService WebWorkerService { get; set; } = default!;

        bool beenInit = false;
        bool busy = true;
        string logMessage = "";
        ElementReference fileInputRef;
        HTMLInputElement? fileInput;
        string outputFileName = "";
        Pipelines? Pipelines = null;
        DepthEstimationPipeline? DepthEstimationPipeline = null;
        File? File = null;
        string? fileObjectUrl = null;
        string? resultObjectUrl = null;
        string? depthObjectUrl = null;
        string depthFileName = "";
        Dictionary<string, ModelLoadProgress> ModelProgresses = new();

        void DownloadDepthmap()
        {
            if (string.IsNullOrEmpty(depthObjectUrl)) return;
            if (string.IsNullOrEmpty(depthFileName)) return;
            DownloadFile(depthObjectUrl, depthFileName);
        }
        void Download2DZImage()
        {
            if (string.IsNullOrEmpty(resultObjectUrl)) return;
            if (string.IsNullOrEmpty(outputFileName)) return;
            DownloadFile(resultObjectUrl, outputFileName);
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
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!beenInit && fileInput == null)
            {
                beenInit = true;
                fileInput = new HTMLInputElement(fileInputRef);
                fileInput.OnChange += FileInput_OnChange;
                busy = false;
                StateHasChanged();
            }
        }
        void Pipeline_OnProgress(ModelLoadProgress obj)
        {
            if (ModelProgresses.TryGetValue(obj.File, out var progress))
            {
                progress.Status = obj.Status;
                if (obj.Progress != null) progress.Progress = obj.Progress;
                if (obj.Total != null) progress.Total = obj.Total;
                if (obj.Loaded != null) progress.Loaded = obj.Loaded;
            }
            else
            {
                ModelProgresses[obj.File] = obj;
            }
            StateHasChanged();
        }
        ActionCallback<ModelLoadProgress> OnProgress => new ActionCallback<ModelLoadProgress>(Pipeline_OnProgress);
        string SelectedModel = Models.First();
        bool UseWebGPU = true;
        static List<string> Models = new List<string>
        {
            "onnx-community/depth-anything-v2-small",
            //"onnx-community/DepthPro-ONNX",
            "Xenova/depth-anything-small-hf",
            //"Xenova/dpt-hybrid-midas",
        };
        string ModelKey = "";
        Dictionary<string, DepthEstimationPipeline> DepthEstimationPipelines = new Dictionary<string, DepthEstimationPipeline>();
        async Task RunIt()
        {
            if (busy) return;
            var key = UseWebGPU ? $"{SelectedModel}+webgpu" : SelectedModel;
            DepthEstimationPipeline = null;
            busy = true;
            StateHasChanged();
            ModelKey = key;
            if (Pipelines == null)
            {
                Log($"Initializing... ", false);
                Pipelines = await Pipelines.Init();
                Log($"Done");
            }
            if (!DepthEstimationPipelines.TryGetValue(key, out var pipeline))
            {
                try
                {
                    Log($"Depth Estimation Pipeline with WebGPU loading... ", false);
                    pipeline = await Pipelines.DepthEstimationPipeline(SelectedModel, new PipelineOptions { Device = UseWebGPU ? "webgpu" : null, OnProgress = OnProgress });
                    DepthEstimationPipelines[key] = pipeline;
                    Log($"Done");
                }
                catch
                {
                    Log($"Error");
                }
                ModelProgresses.Clear();
            }
            DepthEstimationPipeline = pipeline;
            if (DepthEstimationPipeline != null && !string.IsNullOrEmpty(fileObjectUrl) && File != null)
            {
                try
                {
                    await ProcessFile();
                }
                catch { }
            }
            busy = false;
            StateHasChanged();
        }
        async void FileInput_OnChange(Event ev)
        {
            if (!string.IsNullOrEmpty(fileObjectUrl))
            {
                URL.RevokeObjectURL(fileObjectUrl);
                fileObjectUrl = null;
            }
            if (!string.IsNullOrEmpty(resultObjectUrl))
            {
                URL.RevokeObjectURL(resultObjectUrl);
                resultObjectUrl = null;
            }
            if (!string.IsNullOrEmpty(depthObjectUrl))
            {
                URL.RevokeObjectURL(depthObjectUrl);
                depthObjectUrl = null;
            }
            File?.Dispose();
            using var Files = fileInput!.Files;
            File = Files!.FirstOrDefault();
            if (File == null)
            {
                return;
            }
            busy = true;
            StateHasChanged();
            fileObjectUrl = await FileReader.ReadAsDataURLAsync(File);
            var outputFormatExt = "png";
            var ext = File.Name.Split('.').Last();
            var filenameBase = File.Name.Substring(0, File.Name.Length - ext.Length - 1);
            outputFileName = $"{filenameBase}.2DZ.{outputFormatExt}";
            depthFileName = $"{filenameBase}.Z.{outputFormatExt}";
            busy = false;
            StateHasChanged();
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
        async Task ProcessSelectedFile()
        {
            if (!string.IsNullOrEmpty(resultObjectUrl))
            {
                URL.RevokeObjectURL(resultObjectUrl);
                resultObjectUrl = null;
            }
            if (!string.IsNullOrEmpty(depthObjectUrl))
            {
                URL.RevokeObjectURL(depthObjectUrl);
                depthObjectUrl = null;
            }
            StateHasChanged();
            var rgbImage = await HTMLImageElement.CreateFromImageAsync(fileObjectUrl);
            using var depthResult = await DepthEstimationPipeline!.Call(fileObjectUrl);
            using var depthInfo = depthResult.Depth;
            using var depthMapData = depthInfo.Data;
            var rgbWidth = depthInfo.Width;
            var rgbHeight = depthInfo.Height;
            resultObjectUrl = await Create2DZObjectUrl(rgbImage, depthMapData, rgbWidth, rgbHeight);
            depthObjectUrl = await CreateDepthImageObjectUrl(depthMapData, rgbWidth, rgbHeight);
        }
        async Task<string> Create2DZObjectUrl(HTMLImageElement rgbImage, Uint8Array grayscale1BPPData, int width, int height)
        {
            var outWidth = width * 2;
            var outHeight = height;
            var grayscaleDataBytes = grayscale1BPPData.ReadBytes();
            var depthmapRGBABytes = Grayscale1BPPToRGBA(grayscaleDataBytes, width, height);
            using var canvas = new HTMLCanvasElement(outWidth, outHeight);
            using var ctx = canvas.Get2DContext();
            // draw rgb image
            ctx.DrawImage(rgbImage);
            // draw depth map
            ctx.PutImageBytes(depthmapRGBABytes, width, height, width, 0);
            using var blob = await canvas.ToBlobAsync("image/png");
            var ret = URL.CreateObjectURL(blob);
            return ret;
        }
        async Task<string> CreateDepthImageObjectUrl(Uint8Array grayscale1BPPData, int width, int height)
        {
            var grayscaleDataBytes = grayscale1BPPData.ReadBytes();
            var depthmapRGBABytes = Grayscale1BPPToRGBA(grayscaleDataBytes, width, height);
            using var canvas = new HTMLCanvasElement(width, height);
            using var ctx = canvas.Get2DContext();
            ctx.PutImageBytes(depthmapRGBABytes, width, height);
            using var blob = await canvas.ToBlobAsync("image/png");
            var ret = URL.CreateObjectURL(blob);
            return ret;
        }
        byte[] Grayscale1BPPToRGBA(byte[] grayscaleData, int width, int height)
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
        async Task ProcessFile()
        {
            busy = true;
            StateHasChanged();
            try
            {
                Log("Estimating depth... ", false);
                await ProcessSelectedFile();
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
            if (!string.IsNullOrEmpty(resultObjectUrl))
            {
                URL.RevokeObjectURL(resultObjectUrl);
                resultObjectUrl = null;
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
