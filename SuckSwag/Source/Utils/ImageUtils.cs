namespace SuckSwag.Source.Utils
{
    using Accord.Imaging;
    using DataStructures;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Windows;
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

        private static ExhaustiveTemplateMatching recognition = new ExhaustiveTemplateMatching();

        private static MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

        /// <summary>
        /// Finds the best match for the given candidate image against the provided templates.
        /// </summary>
        /// <param name="candidates">The image being compared.</param>
        /// <param name="templates">The templates against which to compare the image.</param>
        /// <returns></returns>
        public static Bitmap BestCandidateMatch(IEnumerable<Bitmap> candidates, params Bitmap[] templates)
        {
            const float similarityThreshold = 0.25f;

            recognition.SimilarityThreshold = similarityThreshold;

            if (candidates == null || candidates.Count() <= 0)
            {
                return null;
            }

            var rankedCandidates = candidates
              // Get the similarity to all template images
              .Select(candidate =>
                  new
                  {
                      bitmap = candidate,
                      matchings = templates.Select(template => recognition.ProcessImage(candidate, template)),
                  })
              .Select(candidate =>
                  new
                  {
                      bitmap = candidate.bitmap,
                      similarity = candidate.matchings.Select(match => match.Count() > 0 ? match[0].Similarity : 0.0f).Max(),
                  })

               .OrderByDescending(candidate => candidate.similarity);

               // Pick the best
            Bitmap bestMatch = rankedCandidates
               // Threshold the similarity
               .Where(board => board.similarity > similarityThreshold)
               .FirstOrDefault()?.bitmap;

            return bestMatch;
        }

        /// <summary>
        /// Finds the best match for the given candidate image against the provided templates.
        /// </summary>
        /// <param name="candidates">The image being compared.</param>
        /// <param name="templates">The templates against which to compare the image.</param>
        /// <returns></returns>
        public static Bitmap BestTemplateMatch(IEnumerable<Bitmap> candidates, params Bitmap[] templates)
        {
            const float similarityThreshold = 0.65f;

            recognition.SimilarityThreshold = similarityThreshold;

            if (candidates == null || candidates.Count() <= 0)
            {
                return null;
            }

            var rankedCandidates = templates
              // Get the similarity to all template images
              .Select(template =>
                  new
                  {
                      bitmap = template,
                      matchings = candidates.Select(candidate => recognition.ProcessImage(candidate, template)),
                  })
              .Select(template =>
                  new
                  {
                      bitmap = template.bitmap,
                      similarity = template.matchings.Select(match => match.Count() > 0 ? match[0].Similarity : 0.0f).Max(),
                  })
               // Pick the best
               .OrderByDescending(template => template.similarity);

            Bitmap bestMatch = rankedCandidates
               // Threshold the similarity
               .Where(square => square.similarity > similarityThreshold)
               .FirstOrDefault()?.bitmap;
            return bestMatch;
        }
        public static bool IsGray(Color c)
        {
            if (Math.Abs(c.R - c.G) > 5 * 2.55) return false;
            if (Math.Abs(c.R - c.B) > 5 * 2.55) return false;
            if (Math.Abs(c.G - c.B) > 5 * 2.55) return false;
            return true;
        }

        public static void ReplaceColorsWithTransparent(Bitmap bitmap, List<Color> ColorsToReplace)
        {
            if (bitmap == null)
            {
                return;
            }

            foreach (Color color in ColorsToReplace)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        if (!IsGray(bitmap.GetPixel(x, y)))
                        {
                            bitmap.SetPixel(x, y, Color.Transparent);
                        }
                    }
                }
            }
        }

        public static Bitmap DiffBitmapsAndPrepareForTemplateMatching(Bitmap bitmapA, Bitmap bitmapB)
        {
            if (bitmapA == null || bitmapB == null || bitmapA.Width != bitmapB.Width || bitmapA.Height != bitmapB.Height)
            {
                return null;
            }

            BitmapData bitmapAData = bitmapA.LockBits(new Rectangle(0, 0, bitmapA.Width, bitmapA.Height), ImageLockMode.ReadOnly, bitmapA.PixelFormat);
            BitmapData bitmapBData = bitmapB.LockBits(new Rectangle(0, 0, bitmapB.Width, bitmapB.Height), ImageLockMode.ReadOnly, bitmapB.PixelFormat);

            int width = bitmapAData.Width;
            int height = bitmapAData.Height;

            int redA, greenA, blueA, redB, greenB, blueB;

            int BppModifier = bitmapA.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;

            int stride = bitmapAData.Stride;
            IntPtr scan0A = bitmapAData.Scan0;
            IntPtr scan0B = bitmapBData.Scan0;

            unsafe
            {
                byte* pA = (byte*)(void*)scan0A;
                byte* pB = (byte*)(void*)scan0B;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = (y * stride) + x * BppModifier;
                        redA = pA[idx + 2];
                        greenA = pA[idx + 1];
                        blueA = pA[idx];
                        redB = pB[idx + 2];
                        greenB = pB[idx + 1];
                        blueB = pB[idx];

                        if (redA == redB && greenA == greenB && blueA == blueB)
                        {
                            pB[idx + 2] = 255;
                            pB[idx + 1] = 255;
                            pB[idx + 0] = 255;
                        }

                        // Force non-gray colors to white
                        if (Math.Abs(pB[idx + 2] - pB[idx + 1]) > 5 * 2.55
                            && Math.Abs(pB[idx + 2] - pB[idx + 0]) > 5 * 2.55
                            && Math.Abs(pB[idx + 1] - pB[idx + 0]) > 5 * 2.55)
                        {
                            pB[idx + 2] = 255;
                            pB[idx + 1] = 255;
                            pB[idx + 0] = 255;
                        }

                        // Polarize black/white
                        byte rgb = (byte)(Math.Round(((pB[idx + 2] + pB[idx + 1] + pB[idx + 0]) / 3.0) / 255) * 255);
                        pB[idx + 2] = rgb;
                        pB[idx + 1] = rgb;
                        pB[idx + 0] = rgb;
                    }
                }
            }

            bitmapA.UnlockBits(bitmapAData);
            bitmapB.UnlockBits(bitmapBData);

            return bitmapB;
        }

        /// <summary>
        /// Converts a bitmap to a purely black and white bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap to polarize.</param>
        /// <returns>A polarized black and white bitmap.</returns>
        public static Bitmap PolarizeBlackWhite(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }

            bitmap = ImageUtils.Clone(bitmap, bitmap.PixelFormat);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color c = bitmap.GetPixel(x, y);

                    if (c.A == 0)
                    {
                        bitmap.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        int rgb = (int)(Math.Round(((c.R + c.G + c.B) / 3.0) / 255) * 255);
                        bitmap.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                    }
                }
            }

            if (bitmap.PixelFormat != PixelFormat.Format24bppRgb)
            {
                bitmap = ImageUtils.Clone(bitmap, PixelFormat.Format24bppRgb);
            }

            return bitmap;
        }

        public static Bitmap Tint(Bitmap sourceBitmap, Color tint)
        {
            if (sourceBitmap == null)
            {
                return null;
            }

            float blueTint = ((float)tint.B / 255.0f);
            float greenTint = ((float)tint.G / 255.0f);
            float redTint = ((float)tint.R / 255.0f);

            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                                    sourceBitmap.Width, sourceBitmap.Height),
                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];


            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);


            sourceBitmap.UnlockBits(sourceData);


            float blue = 0;
            float green = 0;
            float red = 0;


            for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
            {
                blue = pixelBuffer[k] + (255 - pixelBuffer[k]) * blueTint;
                green = pixelBuffer[k + 1] + (255 - pixelBuffer[k + 1]) * greenTint;
                red = pixelBuffer[k + 2] + (255 - pixelBuffer[k + 2]) * redTint;


                if (blue > 255)
                { blue = 255; }


                if (green > 255)
                { green = 255; }


                if (red > 255)
                { red = 255; }


                pixelBuffer[k] = (byte)blue;
                pixelBuffer[k + 1] = (byte)green;
                pixelBuffer[k + 2] = (byte)red;


            }


            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);


            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                    resultBitmap.Width, resultBitmap.Height),
                                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);


            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);


            return resultBitmap;
        }

        public static string ComputeImageHash(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return string.Empty;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Bmp);
                return System.Text.Encoding.UTF8.GetString(ImageUtils.md5.ComputeHash(ms.ToArray()));
            }
        }

        /// <summary>
        /// Clones the given bitmap, and ensures the format is 24bppRgb.
        /// </summary>
        /// <param name="sourceBitmap">The bitmap to clone.</param>
        /// <returns>A cloned bitmap.</returns>
        public static Bitmap Clone(Bitmap sourceBitmap, PixelFormat pixelFormat = PixelFormat.Format24bppRgb)
        {
            if (sourceBitmap == null)
            {
                return null;
            }

            Bitmap clone = new Bitmap(sourceBitmap.Width, sourceBitmap.Height, pixelFormat);

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
            if (bitmapImage == null)
            {
                return null;
            }

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
            if (bitmap == null)
            {
                return null;
            }

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

        public static void CopyBitmapToWritableBitmap(Bitmap bitmap, WriteableBitmap writeableBitmap)
        {
            if (bitmap == null || writeableBitmap == null)
            {
                return;
            }

            if (bitmap.Width != (int)writeableBitmap.Width || bitmap.Height != (int)writeableBitmap.Height)
            {
                bitmap = new Bitmap(bitmap, new System.Drawing.Size((int)writeableBitmap.Width, (int)writeableBitmap.Height));
            }

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                writeableBitmap.Lock();
                CopyMemory(writeableBitmap.BackBuffer, data.Scan0,
                    (writeableBitmap.BackBufferStride * bitmap.Height));
                writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.Width, bitmap.Height));
                writeableBitmap.Unlock();
            }
            finally
            {
                bitmap.UnlockBits(data);
                bitmap.Dispose();
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr dest, IntPtr source, int Length);
    }
    //// End class
}
//// End namespace