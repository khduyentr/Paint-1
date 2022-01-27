using Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paint
{
    class Helper
    {
        private static void SaveRTBAsPNGBMP(RenderTargetBitmap bmp, string filename)
        {
            var enc = new System.Windows.Media.Imaging.PngBitmapEncoder();
            enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bmp));

            using (var stm = System.IO.File.Create(filename))
            {
                enc.Save(stm);
            }
        }
        public static void SaveCanvasToFile(int w, int h, Canvas canvas, string filename)
        {
            Size size = new Size(w, h);
            canvas.Measure(size);
            //Matrix m = PresentationSource.FromVisual(Application.Current.MainWindow).CompositionTarget.TransformToDevice;
            //double dx = m.M11;
            //double dy = m.M22;
            var rtb = new RenderTargetBitmap(
                w, //width
                h, //height
                96, //dpi x
                96, //dpi y
                PixelFormats.Pbgra32 // pixelformat
                );
            rtb.Render(canvas);

            SaveRTBAsPNGBMP(rtb, filename);
        }

        public static List<FontFamily> GetAllFonts()
        {
            List<FontFamily> fontList = new List<FontFamily>();
            foreach (var f in Fonts.SystemFontFamilies)
            {
                fontList.Add(f);
            }
            return fontList;
        }

    }
}
