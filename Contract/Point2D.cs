using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Contract
{
    public class Point2D : IShape
    {
        public double X { get; set; }
        public double Y { get; set; }
        public SolidColorBrush Color { get; set; }
        public string Image { get; set; }
        public Point2D()
        {
            X = 0;
            Y = 0;
            Color = new SolidColorBrush(Colors.Red);
            Image = "";
        }

        public string Name => "Point";

        public void HandleStart(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void HandleEnd(double x, double y)
        {
            X = x;
            Y = y;
        }

        public UIElement Draw()
        {
            Line l = new Line()
            {
                X1 = X,
                Y1 = Y,
                X2 = X,
                Y2 = Y,
                StrokeThickness = 1,
                Stroke = Color,
            };

            return l;
        }

        public IShape NextShape()
        {
            return new Point2D();
        }

        public IShape Clone()
        {
            return new Point2D() { 
                X = this.X,
                Y = this.Y,
                Color = this.Color
            };
        }

        public void ChangePenWidth(int witdh)
        {
            throw new NotImplementedException();
        }
    }
}
