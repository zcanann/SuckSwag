namespace SuckSwag.Source.Utils
{
    using DataStructures;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Forms;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Static class for useful image utilities.
    /// </summary>
    internal static class ImageUtils
    {
        /// <summary>
        /// Cached bitmap mappings stored by this utility.
        /// </summary>
        private static TTLCache<String, Bitmap> bitmapCache = new TTLCache<String, Bitmap>();

        /// <summary>
        /// Converts a bitmap to a purely black and white bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to polarize.</param>
        /// <returns>A polarized black and white bitmap.</returns>
        public static Bitmap PolarizeBlackWhite(Bitmap bitmap)
        {
            bitmap = ImageUtils.Clone(bitmap);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    int rgb = (int)(Math.Round(((c.R + c.G + c.B) / 3.0) / 255) * 255);
                    bitmap.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            }

            return bitmap;
        }

        /// <summary>
        /// Clones the given bitmap, and ensures the format is 24bppRgb.
        /// </summary>
        /// <param name="sourceBitmap">The bitmap to clone.</param>
        /// <returns>A cloned bitmap.</returns>
        public static Bitmap Clone(Bitmap sourceBitmap)
        {
            Bitmap clone = new Bitmap(sourceBitmap.Width, sourceBitmap.Height, PixelFormat.Format24bppRgb);

            using (Graphics gr = Graphics.FromImage(clone))
            {
                gr.DrawImage(sourceBitmap, new Rectangle(0, 0, clone.Width, clone.Height));
            }

            return clone;
        }

        /// <summary>
        /// Copies a section of a given bitmap.
        /// </summary>
        /// <param name="sourceBitmap">The source bitmap.</param>
        /// <param name="section">The section to copy.</param>
        /// <returns>The section of the given bitmap.</returns>
        public static Bitmap Copy(Bitmap sourceBitmap, Rectangle section)
        {
            // Create the new bitmap and associated graphics object
            Bitmap bitmap = new Bitmap(section.Width, section.Height, PixelFormat.Format24bppRgb);
            Graphics graphics = Graphics.FromImage(bitmap);

            // Draw the specified section of the source bitmap to the new one
            graphics.DrawImage(sourceBitmap, 0, 0, section, GraphicsUnit.Pixel);

            // Clean up
            graphics.Dispose();

            // Return the bitmap
            return bitmap;
        }

        /// <summary>
        /// Collects a screen shot of the entire screen.
        /// </summary>
        /// <returns>The screen shot of the entire screen.</returns>
        public static Bitmap CollectScreenCapture()
        {
            Bitmap screenshot = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height, PixelFormat.Format24bppRgb);

            Graphics screenGraph = Graphics.FromImage(screenshot);

            screenGraph.CopyFromScreen(
                SystemInformation.VirtualScreen.X,
                SystemInformation.VirtualScreen.Y,
                0,
                0,
                SystemInformation.VirtualScreen.Size,
                CopyPixelOperation.SourceCopy);

            return screenshot;
        }

        /// <summary>
        /// Loads an image from the given uri.
        /// </summary>
        /// <param name="uri">The uri specifying from where to load the image.</param>
        /// <returns>The bitmap image loaded from the given uri.</returns>
        public static BitmapImage LoadImage(String uri)
        {
            BitmapImage bitmapImage = new BitmapImage();

            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(uri);
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        /// <summary>
        /// Converts a <see cref="BitmapImage"/> to a <see cref="Bitmap"/>.
        /// </summary>
        /// <param name="bitmapImage">The bitmap image to convert.</param>
        /// <returns>The resulting bitmap.</returns>
        public static Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            String uri = bitmapImage?.UriSource?.AbsoluteUri;

            if (ImageUtils.bitmapCache.Contains(uri))
            {
                Bitmap result;

                if (ImageUtils.bitmapCache.TryGetValue(uri, out result))
                {
                    return result;
                }
            }

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);
                bitmap?.MakeTransparent();
                ImageUtils.bitmapCache.Add(uri, bitmap);

                return new Bitmap(bitmap);
            }
        }

        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
    }
    //// End class
}
//// End namespace