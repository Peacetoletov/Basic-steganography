using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Drawing;


namespace Steganography
{
    public class Image
    {
        List<List<Color>> mainImagePixels;
        List<List<Color>> hiddenImagePixels;


        public void LoadMainImage(string path) {
            this.mainImagePixels = LoadImage(path);
        }

        public void LoadHiddenImage(string path) {
            this.hiddenImagePixels = LoadImage(path);
        }

        private List<List<Color>> LoadImage(string path) {

            /*
            Note: using Bitmap requires the installation of System.Drawing.Common package
            (dotnet add package System.Drawing.Common).
            Note 2: Bitmap is only supported on Windows.
            */

            Bitmap img = new Bitmap(path);
            List<List<Color>> imagePixels = new List<List<Color>>();
            for (int y = 0; y < img.Height; y++) {
                List<Color> row = new List<Color>();
                for (int x = 0; x < img.Width; x++) {
                    Color pixel = img.GetPixel(x, y);
                    /*
                    if (pixel.R == 0 && pixel.G == 0 && pixel.B == 0) {
                        // Pixel is background
                        row.Add(0);
                    } else {
                        // Pixel is foreground
                        row.Add(1);
                    }
                    */
                    row.Add(pixel);
                }
                imagePixels.Add(row);
            } 
            return imagePixels;
        }

        public void CreateMergedImage(string path) {
            /*
            For each pixel:
            Remove the last 2 bits of each color channel in the main image. Calculate a 6-bit greyscale
            value of the pixel of the hidden image, then distribute this value across the last 2 bits of
            all 3 color channels of the main image. Save this image into path.

            Note: both images need to have the same dimensions.
            */

            if (mainImagePixels.Count != hiddenImagePixels.Count || mainImagePixels[0].Count != hiddenImagePixels[0].Count) {
                throw new Exception("Error: Main image and hidden image must have the same dimensions!");
            }

            int width = mainImagePixels[0].Count;
            int height = mainImagePixels.Count;
            Bitmap bm = new Bitmap(width, height);
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    byte grayscale = ConvertPixelToSixBitGrayscale(hiddenImagePixels[y][x]);
                    //byte r = (byte) (SetLastTwoBitsToZero(mainImagePixels[y][x].R) + ((grayscale & (0b11 << 4)) >> 4));
                    //byte g = (byte) (SetLastTwoBitsToZero(mainImagePixels[y][x].G) + ((grayscale & (0b11 << 2)) >> 2));
                    byte r = (byte) (SetLastTwoBitsToZero(mainImagePixels[y][x].R) + ((grayscale & 0b00110000) >> 4));
                    byte g = (byte) (SetLastTwoBitsToZero(mainImagePixels[y][x].G) + ((grayscale & 0b00001100) >> 2));
                    byte b = (byte) (SetLastTwoBitsToZero(mainImagePixels[y][x].B) + (grayscale & 0b00000011));
                    bm.SetPixel(x, y, Color.FromArgb(0, r, g, b));
                }
            }
            bm.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
        }

        private byte ConvertPixelToSixBitGrayscale(Color pixel) {
            byte grayscaleSixBit = (byte) ((pixel.R + pixel.G + pixel.B) / 12);
            // ^ dividing by 3 to get the average, then dividing by 4 because the range is reduced 
            // from 256 values to 64
            return grayscaleSixBit;
        }

        private byte SetLastTwoBitsToZero(int number) {
            return (byte) ((number >> 2) << 2);
        }

        public void RevealHiddenImage(string originalImagePath, string revealedImagePath) {
            List<List<Color>> originalImagePixels = LoadImage(originalImagePath);
            int width = originalImagePixels[0].Count;
            int height = originalImagePixels.Count;
            Bitmap bm = new Bitmap(width, height);
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    byte grayscaleSixBit = 0;
                    grayscaleSixBit += (byte) ((originalImagePixels[y][x].R & 0b00000011) << 4);
                    grayscaleSixBit += (byte) ((originalImagePixels[y][x].G & 0b00000011) << 2);
                    grayscaleSixBit += (byte) (originalImagePixels[y][x].B & 0b00000011);
                    byte grayscaleEightBit = (byte) (grayscaleSixBit << 2);
                    bm.SetPixel(x, y, Color.FromArgb(0, grayscaleEightBit, grayscaleEightBit, grayscaleEightBit));
                }
            }
            bm.Save(revealedImagePath, System.Drawing.Imaging.ImageFormat.Bmp);
        }
    }
}