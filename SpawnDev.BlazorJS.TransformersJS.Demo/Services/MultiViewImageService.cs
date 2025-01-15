using SpawnDev.BlazorJS.JSObjects;
using SpawnDev.BlazorJS.TransformersJS.Demo.Renderers;

namespace SpawnDev.BlazorJS.TransformersJS.Demo.Services
{
    public class MultiViewImageService
    {
        BlazorJSRuntime JS;
        DepthEstimationService DepthEstimationService;
        public MultiViewImageService(BlazorJSRuntime js, DepthEstimationService depthEstimationService)
        {
            JS = js;
            DepthEstimationService = depthEstimationService;

        }
        AnaglyphRenderer? anaglyphRenderer = null;
        Dictionary<string, HTMLImageElement> Results = new Dictionary<string, HTMLImageElement> ();
        public async Task<string> ImageToAnaglyph(string source, string model = "onnx-community/depth-anything-v2-small", bool useWebGPU = true, float focusDepth = 0.5f, float level3D = 1)
        {
            if (!Results.TryGetValue(source, out var imageWithDepth))
            {
                // get image
                using var image = await HTMLImageElement.CreateFromImageAsync(source);
                // get the depth estimation pipeline
                var DepthEstimationPipeline = await DepthEstimationService.GetDepthEstimationPipeline(model, useWebGPU);
                // generate the depth map
                using var depthResult = await DepthEstimationPipeline!.Call(source);
                using var depthInfo = depthResult.Depth;
                using var depthMapData = depthInfo.Data;
                var depthWidth = depthInfo.Width;
                var depthHeight = depthInfo.Height;
                // create 2D+Z image object url
                var imageWithDepthObjectUrl = await Create2DZObjectUrl(image, depthMapData, depthWidth, depthHeight);
                imageWithDepth = await HTMLImageElement.CreateFromImageAsync(imageWithDepthObjectUrl);
                Results[source] = imageWithDepth;
            }
            // use WebGL to convert 2D+Z to anaglyph
            anaglyphRenderer ??= new AnaglyphRenderer();
            anaglyphRenderer.Level3D = level3D;
            anaglyphRenderer.Focus3D = focusDepth;
            anaglyphRenderer.SetInput(imageWithDepth, "2dz");
            anaglyphRenderer.Render();
            var temp = await anaglyphRenderer.ToObjectUrl();
            return temp;
        }
        public async Task<HTMLImageElement> ImageTo2DZImage(HTMLImageElement image, string model = "onnx-community/depth-anything-v2-small", bool useWebGPU = true)
        {
            var source = image.Src;
            if (!Results.TryGetValue(source, out var imageWithDepth))
            {
                // get the depth estimation pipeline
                var DepthEstimationPipeline = await DepthEstimationService.GetDepthEstimationPipeline(model, useWebGPU);
                // generate the depth map
                using var depthResult = await DepthEstimationPipeline!.Call(source);
                using var depthInfo = depthResult.Depth;
                using var depthMapData = depthInfo.Data;
                var depthWidth = depthInfo.Width;
                var depthHeight = depthInfo.Height;
                // create 2D+Z image object url
                var imageWithDepthObjectUrl = await Create2DZObjectUrl(image, depthMapData, depthWidth, depthHeight);
                imageWithDepth = await HTMLImageElement.CreateFromImageAsync(imageWithDepthObjectUrl);
                Results[source] = imageWithDepth;
            }
            return imageWithDepth;
        }
        public async Task<HTMLImageElement> ImageTo2DZImage(string source, string model = "onnx-community/depth-anything-v2-small", bool useWebGPU = true)
        {
            if (!Results.TryGetValue(source, out var imageWithDepth))
            {
                // get image
                using var image = await HTMLImageElement.CreateFromImageAsync(source);
                // get the depth estimation pipeline
                var DepthEstimationPipeline = await DepthEstimationService.GetDepthEstimationPipeline(model, useWebGPU);
                // generate the depth map
                using var depthResult = await DepthEstimationPipeline!.Call(source);
                using var depthInfo = depthResult.Depth;
                using var depthMapData = depthInfo.Data;
                var depthWidth = depthInfo.Width;
                var depthHeight = depthInfo.Height;
                // create 2D+Z image object url
                var imageWithDepthObjectUrl = await Create2DZObjectUrl(image, depthMapData, depthWidth, depthHeight);
                imageWithDepth = await HTMLImageElement.CreateFromImageAsync(imageWithDepthObjectUrl);
                Results[source] = imageWithDepth;
            }
            return imageWithDepth;
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
    }
}
