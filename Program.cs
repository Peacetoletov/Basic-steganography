using System;

namespace Steganography
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Image image = new Image();
            
            image.LoadMainImage("myMain.jpg");
            image.LoadHiddenImage("mySecret.jpg");
            image.CreateMergedImage("myMerged.png");
            image.RevealHiddenImage("myMerged.png", "myRevealed.png");
        }
    }
}
