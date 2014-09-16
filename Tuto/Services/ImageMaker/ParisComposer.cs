using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.TutoServices
{
    public class BasicProgrammingComposer
    {
        const int margin = 60;
        const int lineSpacing = 40;
        const string FontFamily = "Segoe UI";
        const int FontSize = 25;

        Func<string,int> GetHeightMeasurer(Graphics g, Font font, int width)
        {
            return s => (int)g.MeasureString(s,font,width).Height;
        }

        public System.Drawing.Bitmap Compose(Model.GlobalData globalData, string[] captions)
        {
            Bitmap result = new Bitmap(1280, 720);
            var graphics = Graphics.FromImage(result);
            graphics.Clear(Color.White);

            var logo=(Bitmap)Image.FromFile(globalData.TitleImage);
            var k = (double)(result.Height - 2 * margin) / logo.Height;

            var logoRect = new Rectangle(margin, margin, (int)(k * logo.Width), (int)(k * logo.Height));

            graphics.DrawImage(logo,
                logoRect,
                new Rectangle(0,0,logo.Width,logo.Height),
                GraphicsUnit.Pixel);

            var textRect = new Rectangle(
                logoRect.Right + margin, 
                margin, 
                result.Width - logoRect.Width - 3 * margin, 
                result.Height - 2 * margin);

            

            var font = new Font(FontFamily, FontSize);
            var measuge=GetHeightMeasurer(graphics,font,textRect.Width);
            var heights=captions.Select(measuge).ToList();
            var actualTextHeight=heights.Aggregate((total, x) => total + x + lineSpacing);
            var y = (textRect.Height - actualTextHeight) / 2;

            var format=new StringFormat { Alignment = StringAlignment.Center };

            foreach (var e in captions)
            {
                
                graphics.DrawString(e, font, Brushes.Black, 
                    new Rectangle(textRect.Left, textRect.Top+y, textRect.Width, 1000),
                     format);
                y += measuge(e) + lineSpacing;
            }
            


            return result;
        }
    }
}
