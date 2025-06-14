using SpawnDev.BlazorJS.JSObjects;

namespace SpawnDev.BlazorJS.TransformersJS
{
    /// <summary>
    /// Adds extensions for HTMLImageElement to check if an image is usable.
    /// </summary>
    public static class HTMLImageElementExtensions
    {
        /// <summary>
        /// Check if an image is usable by drawing it to an offscreen canvas and trying to read back a pixel<br/>
        /// Images that would cause a tainted canvas will return false
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static bool IsImageUsable(this HTMLImageElement image)
        {
            if (image == null) return false;
            if (!image.Complete || image.NaturalHeight == 0 || image.NaturalWidth == 0) return false;
            try
            {
                using var canvas = new OffscreenCanvas(1, 1);
                using var ctx = canvas.Get2DContext();
                ctx.DrawImage(image, 0, 0);
                using var pixel = ctx.GetImageData(0, 0, 1, 1);
                return true;
            }
            catch { }
            return false;
        }
        /// <summary>
        /// If the specified HTMLImageElement is not "tainted" it is returned, otherwise a couple attempts are made to acquire a non-tainted copy of the image.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static async Task<HTMLImageElement?> GetUsableImage(this HTMLImageElement image)
        {
            if (image == null) return null;
            if (image.IsImageUsable()) return image;
            var source = image.Src;
            if (string.IsNullOrEmpty(source)) return null;
            // try using an image we load ourselves using crossOrigin = "anonymous"
            try
            {
                var altImage = await HTMLImageElement.CreateFromImageAsync(source, "anonymous");
                if (!altImage.IsImageUsable() || altImage.NaturalWidth != image.NaturalWidth || altImage.NaturalHeight != image.NaturalHeight)
                {
                    altImage.Dispose();
                    altImage = await HTMLImageElement.CreateFromImageAsync(source, "user-credentials");
                    if (!altImage.IsImageUsable() || altImage.NaturalWidth != image.NaturalWidth || altImage.NaturalHeight != image.NaturalHeight)
                    {
                        altImage.Dispose();
                        return null;
                    }
                    // successfully loaded image
                    return altImage;
                }
                else
                {
                    // successfully loaded image
                    return altImage;
                }
            }
            catch { }
            return null;
        }
    }
}
