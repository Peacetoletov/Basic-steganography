using System;
using System.Diagnostics;


namespace Steganography
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Image image = new Image();
            
            /*
            image.LoadMainImage("Yet unused\\Memes\\fi.png");
            image.LoadHiddenImage("Yet unused\\Images\\5.jpg");
            image.CreateMergedImage("Discord posts\\Ready\\5\\fi.png");
            image.RevealHiddenImage("Discord posts\\Ready\\5\\fi.png", "Discord posts\\Ready\\5\\revealed.png");
            */

            image.RevealHiddenImage("Discord posts\\Ready\\5\\fi.png");

        }
    }
}
