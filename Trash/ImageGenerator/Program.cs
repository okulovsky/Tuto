using System;
using System.Drawing;
using System.IO;

namespace ImageGenerator
{
	class Program
	{
        static void GenerateImage(string title, string subtitle, string sourceImagePath, int widht, int height, string output)
        {
            var bmp = new Bitmap(widht, height);
            var img = new Bitmap(sourceImagePath);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));  // Don't do anything with image, just place it here as 1:1
                var xOffset = img.Width;

                // draw upper title
                using (var font = new Font("Arial", 36, FontStyle.Bold, GraphicsUnit.Point))
                {
                    var rect = new Rectangle(xOffset, 0, widht - xOffset, height / 2);

                    var stringFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

                    g.DrawString(title, font, Brushes.Black, rect, stringFormat);
                }
                // draw lower subtitle
                using (var font = new Font("Arial", 26, FontStyle.Regular, GraphicsUnit.Point))
                {
                    var rect = new Rectangle(xOffset, height / 2, widht - xOffset, height / 2);

                    var stringFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

                    g.DrawString(subtitle, font, Brushes.Black, rect, stringFormat);
                }

            }

            bmp.Save(output);
        }


		static void Main(string[] args)
		{
            if (args.Length != 3)
			{
				Console.WriteLine("ImageGenerator [folder] [width] [height]");
				Console.WriteLine("Example: ImageGenerator C:\\image.png 1 3 1280 720 d:\\picture.png");
				return;
			}
            int width=0; 
            int height=0;
            if (!int.TryParse(args[1],out width) || !int.TryParse(args[2], out height))
            {
                Console.WriteLine("Invalid parameters");
                return;
            }

            var title=File.ReadAllText(args[0]+"\\..\\titles.txt");
            var subtitles=File.ReadAllLines(args[0]+"\\titles.txt");
            for(int i=0;i<subtitles.Length;i++)
            {
                GenerateImage(
                    title,
                    subtitles[i],
                    args[0]+"\\..\\picture.png",
                    width,
                    height,
                    args[0]+"\\intro_for_"+(i).ToString()+".png");
            }
		}
	}
}
