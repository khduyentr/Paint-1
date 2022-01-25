using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Contract
{
    public class Point2DData {
        public double X { get; set; }
        public double Y { get; set; }
        public int PenWidth { get; set; }
        public string FillColor { get; set; }
        public string Color { get; set; }
    }
    public class Point2D : IShape
    {
        public double X { get; set; }
        public double Y { get; set; }
        private int _penWidth = 1;
        public SolidColorBrush FillColor { get; set; }
        public SolidColorBrush Color { get; set; }
        public string Image { get; set; }
        public Point2D()
        {
            X = 0;
            Y = 0;
            Color = new SolidColorBrush(Colors.Red);
            FillColor = new SolidColorBrush(Colors.Red);
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
                StrokeThickness = _penWidth,
                Stroke = Color,
                Fill = FillColor,
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
                _penWidth = this._penWidth,
                Color = this.Color,
                FillColor = this.FillColor
            };
        }

        public void ChangePenWidth(int witdh)
        {
            _penWidth = witdh;
        }

        public void ChangeStrokeDash(List<double> strokeDash)
        {
            throw new NotImplementedException();
        }

        public string ToJson()
        {
            Point2DData data = new Point2DData();
            data.X = this.X;
            data.Y = this.Y;
            data.PenWidth = _penWidth;
            data.Color = this.Color.ToString();
            data.FillColor = this.FillColor.ToString();
            string json = JsonSerializer.Serialize(data);
            return json;
        }

        public IShape Parse(string json)
        {
            Point2DData pointData = (Point2DData)JsonSerializer.Deserialize(json, typeof(Point2DData));
            Color c = (Color)ColorConverter.ConvertFromString(pointData.Color);
            Color fc = (Color)ColorConverter.ConvertFromString(pointData.FillColor);
            Point2D result = new Point2D()
            {
                X = pointData.X,
                Y = pointData.Y,
                _penWidth = pointData.PenWidth,
                Color = new SolidColorBrush(c),
                FillColor = new SolidColorBrush(fc)
            };
            return result;
        }
    }
}
