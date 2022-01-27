using Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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

        public static RenderTargetBitmap ConvertCanvasToBitmap(Canvas canvas)
        {
            Size size = new Size(canvas.ActualWidth, canvas.ActualHeight);
            canvas.Measure(size);
            var rtb = new RenderTargetBitmap(
                (int)canvas.ActualWidth,
                (int)canvas.ActualHeight,
                96,
                96,
                PixelFormats.Pbgra32
                );
            rtb.Render(canvas);
            return rtb;
        }

        public static Color GetColorAt(RenderTargetBitmap rtb, Point p)
        {
            CroppedBitmap Cropped = new CroppedBitmap(rtb, new Int32Rect((int)p.X, (int)p.Y, 1, 1));
            byte[] Pixels = new byte[4];
            Cropped.CopyPixels(Pixels, 4, 0);
            return Color.FromArgb(Pixels[3], Pixels[2], Pixels[1], Pixels[0]);
        }

        public static bool IsInPointArr(List<Point> arr, Point p)
        {
            foreach(var p_a in arr)
            {
                if((int)p_a.X == (int)p.X && (int)p_a.Y == (int)p.Y)
                {
                    return true;
                }
            }
            return false;
        }
       
        public static BrushStroke FloodFill(Canvas canvas, RenderTargetBitmap rtb, List<Layer> layers, Point targetPoint, Color newColor)
        {
            BrushStroke result = new BrushStroke();
            Color targetColor = GetColorAt(rtb, targetPoint);
            if (targetColor.ToString().Equals(newColor.ToString())){
                return result;
            }
            result.HandleStart(targetPoint.X, targetPoint.Y);
            Stack<Point> pixels = new Stack<Point>();
            List<Point> arr = new List<Point>();
            pixels.Push(new Point(targetPoint.X, targetPoint.Y));
            arr.Add(new Point(targetPoint.X, targetPoint.Y));
            result.Color = new SolidColorBrush(newColor);
            
            while (pixels.Count != 0)
            {
                Point temp = pixels.Pop();
                int y1 = (int)temp.Y;
                string t = GetColorAt(rtb, new Point(temp.X, y1)).ToString();
                while (y1 >= 0 && GetColorAt(rtb, new Point(temp.X, y1)).Equals(targetColor))
                {
                    y1--;
                }
                y1++;
                bool spanLeft = false;
                bool spanRight = false;
                while (y1 < rtb.Height && GetColorAt(rtb, new Point(temp.X, y1)).Equals(targetColor) && !IsInPointArr(arr, new Point(temp.X, y1)))
                {
                    //bmp.SetPixel(temp.X, y1, replacementColor);

                    result.HandleEnd(temp.X, y1);
                    arr.Add(new Point(temp.X, y1));
                    Color Xt1 = GetColorAt(rtb, new Point(temp.X - 1, y1));
                    Color Xc1 = GetColorAt(rtb, new Point(temp.X + 1, y1));
                    bool isInt1 = IsInPointArr(arr, new Point(temp.X - 1, y1));
                    bool isInc1 = IsInPointArr(arr, new Point(temp.X + 1, y1));
                    bool colorCmp1 = Xt1.ToString().Equals(targetColor.ToString());
                    bool colorCmp2 = Xc1.ToString().Equals(targetColor.ToString());
                    if (!spanLeft && temp.X > 0 && colorCmp1 && !isInt1)
                    {
                        pixels.Push(new Point(temp.X - 1, y1));
                        spanLeft = true;
                    }
                    else if (spanLeft && temp.X - 1 <= 0 && (!colorCmp1 || isInt1))
                    {
                        spanLeft = false;
                    }
                    if (!spanRight && temp.X < rtb.Width - 1 && colorCmp2 && !isInc1)
                    {
                        pixels.Push(new Point(temp.X + 1, y1));
                        spanRight = true;
                    }
                    else if (spanRight && temp.X >= rtb.Width && (!colorCmp2 || isInc1))
                    {
                        spanRight = false;
                    }
                    y1++;
                }
            }
            return result;
        }

    }
}
