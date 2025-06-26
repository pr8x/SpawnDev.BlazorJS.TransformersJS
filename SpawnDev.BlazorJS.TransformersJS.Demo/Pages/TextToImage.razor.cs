using Microsoft.AspNetCore.Components;
using SpawnDev.BlazorJS.JSObjects;
using File = SpawnDev.BlazorJS.JSObjects.File;


namespace SpawnDev.BlazorJS.TransformersJS.Demo.Pages
{
    public partial class TextToImage
    {
        [Inject]
        BlazorJSRuntime JS { get; set; } = default!;

        public string TextBoxValue { get; set; } = "";

        bool beenInit = false;
        bool busy = false;
        string logMessage = "";
        string outputFileName = "depthmap.png";
        Transformers? Transformers = null;
        //DepthEstimationPipeline? DepthEstimationPipeline = null;
        string? depthObjectUrl = null;

        async Task ProcessText()
        {
            var inputText = "A cute and adorable baby fox with big brown eyes, autumn leaves in the background enchanting,immortal,fluffy, shiny mane,Petals,fairyism,unreal engine 5 and Octane Render,highly detailed, photorealistic, cinematic, natural colors.";
            var conversation = new { Role = "User", Content = inputText };
            //var inputs = await processor.Call(conversation, new { chat_template = "text_to_image" });
            //var numImageTokens = processor.NumImageTokens;
            //var outputs = await model.GenerateImages
        }

        Dictionary<string, ModelLoadProgress> ModelProgresses = new();
        void Pipeline_OnProgress(ModelLoadProgress obj)
        {
            var key = $"";
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
            if (obj.Status == "done")
            {
                ModelProgresses.Remove(obj.File);
            }
            StateHasChanged();
        }
        ActionCallback<ModelLoadProgress> OnProgress => new ActionCallback<ModelLoadProgress>(Pipeline_OnProgress);
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!beenInit && !busy && Transformers == null)
            {
                busy = true;
                Log($"Transformers initializing... ", false);
                Transformers = await Transformers.Init();
                Log($"Done");
                busy = false;
                StateHasChanged();
            }
        }
        string Kitti = "onnx-community/dpt-dinov2-small-kitti";
        /// <summary>
        /// Really large... fails to load
        /// </summary>
        string JanusModelId = "onnx-community/Janus-1.3B-ONNX";
        AutoProcessor? processor = null;
        MultiModalityCausalLM? model = null;
        async Task LoadModel()
        {
            if (Transformers == null) return;
            busy = true;
            try
            {
                var fp16Supported = false;

                var opts = fp16Supported ? new FromPretrainedSubOptions
                {
                    PrepareInputsEmbeds = "q4",
                    LanguageModel = "q4f16",
                    LmHead = "fp16",
                    GenHead = "fp16",
                    GenImgEmbeds = "fp16",
                    ImageDecode = "fp32",
                } : new FromPretrainedSubOptions
                {
                    PrepareInputsEmbeds = "fp32",
                    LanguageModel = "q4",
                    LmHead = "fp32",
                    GenHead = "fp32",
                    GenImgEmbeds = "fp32",
                    ImageDecode = "fp32",
                };
                var deviceOpts = new FromPretrainedSubOptions
                {
                    PrepareInputsEmbeds = "wasm",
                    LanguageModel = "webgpu",
                    LmHead = "webgpu",
                    GenHead = "webgpu",
                    GenImgEmbeds = "webgpu",
                    ImageDecode = "webgpu",
                };
                var options = new FromPretrainedOptions
                {
                    OnProgress = OnProgress,
                    Device = deviceOpts,
                    Dtype = opts,
                };
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
            }
            beenInit = true;
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

        public void Dispose()
        {
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
            var width = image.NaturalWidth;
            var height = image.NaturalHeight;
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
