using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Image = System.Drawing.Image;

namespace ImageProcessing.Services
{
    /// <summary>
    /// Defines the <see cref="MotionDetector" />
    /// </summary>
    public class MotionDetector
    {
        /// <summary>
        /// Defines the grayscaleFilter
        /// </summary>
        private readonly IFilter grayscaleFilter = new GrayscaleBT709();

        /// <summary>
        /// Defines the backgroundFrame
        /// </summary>
        private Bitmap backgroundFrame;

        /// <summary>
        /// Defines the bitmapData
        /// </summary>
        private BitmapData bitmapData;

        /// <summary>
        /// Defines the counter
        /// </summary>
        private int counter;

        /// <summary>
        /// Defines the differenceFilter
        /// </summary>
        private readonly Difference differenceFilter = new Difference();

        /// <summary>
        /// Defines the edgesFilter
        /// </summary>
        private readonly IFilter edgesFilter = new Edges();

        /// <summary>
        /// Defines the extrachChannel
        /// </summary>
        private readonly IFilter extrachChannel = new ExtractChannel(RGB.G);

        /// <summary>
        /// Defines the height
        /// </summary>
        private int height;// image height

        /// <summary>
        /// Defines the mergeFilter
        /// </summary>
        private readonly Merge mergeFilter = new Merge();

        /// <summary>
        /// Defines the moveTowardsFilter
        /// </summary>
        private readonly MoveTowards moveTowardsFilter = new MoveTowards();

        /// <summary>
        /// Defines the openingFilter
        /// </summary>
        private readonly IFilter openingFilter = new Opening();

        /// <summary>
        /// Defines the pixelsChanged
        /// </summary>
        private int pixelsChanged;

        /// <summary>
        /// Defines the replaceChannel
        /// </summary>
        private readonly ReplaceChannel replaceChannel = new ReplaceChannel(RGB.R, null);

        /// <summary>
        /// Defines the thresholdFilter
        /// </summary>
        private readonly Threshold thresholdFilter = new Threshold(15);

        /// <summary>
        /// Defines the width
        /// </summary>
        private int width;// image width

        /// <summary>
        /// The ProcessFrame
        /// </summary>
        /// <param name="image">The image<see cref="Bitmap"/></param>
        public Bitmap ProcessFrame(Bitmap image)
        {
            if (backgroundFrame == null)
            {
                // create initial backgroung image
                backgroundFrame = grayscaleFilter.Apply(image);

                // get image dimension
                width = image.Width;
                height = image.Height;

                // just return for the first time
                return null;
            }

            // apply the the grayscale file
            var tmpImage = grayscaleFilter.Apply(image);


            if (++counter == 2)
            {
                counter = 0;

                // move background towards current frame
                moveTowardsFilter.OverlayImage = tmpImage;
                moveTowardsFilter.ApplyInPlace(backgroundFrame);
            }

            // set backgroud frame as an overlay for difference filter
            differenceFilter.OverlayImage = backgroundFrame;

            // lock temporary image to apply several filters
            bitmapData = tmpImage.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            // apply difference filter
            differenceFilter.ApplyInPlace(bitmapData);
            // apply threshold filter
            thresholdFilter.ApplyInPlace(bitmapData);

            pixelsChanged = CalculateWhitePixels(bitmapData);

            var tmpImage2 = openingFilter.Apply(bitmapData);

            // unlock temporary image
            tmpImage.UnlockBits(bitmapData);
            tmpImage.Dispose();

            // apply edges filter
            var tmpImage2b = edgesFilter.Apply(tmpImage2);
            tmpImage2.Dispose();

            // extract red channel from the original image
            var redChannel = extrachChannel.Apply(image);

            //  merge red channel with moving object borders
            mergeFilter.OverlayImage = tmpImage2b;
            var tmpImage3 = mergeFilter.Apply(redChannel);
            redChannel.Dispose();
            tmpImage2b.Dispose();

            // replace red channel in the original image
            replaceChannel.ChannelImage = tmpImage3;
            var tmpImage4 = replaceChannel.Apply(image);
            tmpImage3.Dispose();

            image.Dispose();
            image = tmpImage4;
            return image;
        }

        public  Bitmap ConvertToBitmap(string fileName)
        {
            Bitmap bitmap;
            using (Stream bmpStream = System.IO.File.Open(fileName, System.IO.FileMode.Open))
            {
                Image image = Image.FromStream(bmpStream);

                bitmap = new Bitmap(image);

            }
            return bitmap;
        }
        /// <summary>
        /// The CalculateWhitePixels
        /// </summary>
        /// <param name="bitmapData">The bitmapData<see cref="BitmapData"/></param>
        /// <returns>The <see cref="int"/></returns>
        private int CalculateWhitePixels(BitmapData bitmapData)
        {
            var count = 0;
            var offset = bitmapData.Stride - width;

            unsafe
            {
                var ptr = (byte*)bitmapData.Scan0.ToPointer();

                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++, ptr++) count += *ptr >> 7;
                    ptr += offset;
                }
            }

            return count;
        }
    }
}
