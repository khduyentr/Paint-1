using Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Star2D
{
    public class Star2DData
    {
        public string LeftTop { get; set; }
        public string RightBottom { get; set; }
        public int PenWidth { get; set; }
        public List<double> StrokeDash { get; set; }
        public string Color { get; set; }
        public string FillColor { get; set; }
    }
    public class Star2D : IShape, INotifyPropertyChanged
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();
        private int _penWidth = 1;
        private List<double> _strokeDash = new List<double>() { 0 };
        public SolidColorBrush Color { get; set; }
        public string Name => "Star";
        public SolidColorBrush FillColor { get; set; }
        public string Image { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Star2D()
        {
            Color = new SolidColorBrush(Colors.Black);
            FillColor = new SolidColorBrush(Colors.Transparent);
            Image = "/Star2D;Component/images/star.png";
        }
        public void HandleStart(double x, double y)
        {
            _leftTop = new Point2D() { X = x, Y = y };
        }

        public void HandleEnd(double x, double y)
        {
            _rightBottom = new Point2D() { X = x, Y = y };
        }

        public UIElement Draw()
        {
            double left = (_rightBottom.X > _leftTop.X) ? _leftTop.X : _rightBottom.X;
            double top = (_rightBottom.Y > _leftTop.Y) ? _leftTop.Y : _rightBottom.Y;
            double w = Math.Abs(_rightBottom.X - _leftTop.X);
            double h = Math.Abs(_rightBottom.Y - _leftTop.Y);

            Point p1 = new Point(0, 2 * h / 5);
            Point p2 = new Point(19 * w / 50, 19 * h / 50);
            Point p3 = new Point(w / 2, 0);
            Point p4 = new Point(31 * w / 50, 19 * h / 50);
            Point p5 = new Point(w, 2 * h / 5);
            Point p6 = new Point(7 * w / 10, 3 * h / 5);
            Point p7 = new Point(4 * w / 5, h);
            Point p8 = new Point(w / 2, 3 * h / 4);
            Point p9 = new Point(w / 5, h);
            Point p10 = new Point(w * 3 / 10, 3 * h / 5);
            Polygon poly = new Polygon()
            {
                StrokeDashArray = new DoubleCollection(_strokeDash),
                StrokeThickness = _penWidth,
                Stroke = Color,
                Fill = FillColor,
            };
            PointCollection polygonPoints = new PointCollection();
            polygonPoints.Add(p1);
            polygonPoints.Add(p2);
            polygonPoints.Add(p3);
            polygonPoints.Add(p4);
            polygonPoints.Add(p5);
            polygonPoints.Add(p6);
            polygonPoints.Add(p7);
            polygonPoints.Add(p8);
            polygonPoints.Add(p9);
            polygonPoints.Add(p10);
            poly.Points = polygonPoints;
            Canvas.SetLeft(poly, left);
            Canvas.SetTop(poly, top);
            return poly;
        }

        public IShape NextShape()
        {
            return new Star2D();
        }

        public IShape Clone()
        {
            return new Star2D()
            {
                _leftTop = (Point2D)this._leftTop.Clone(),
                _rightBottom = (Point2D)this._rightBottom.Clone(),
                Color = this.Color,
                FillColor = this.FillColor,
                _penWidth = this._penWidth,
                _strokeDash = new List<double>(this._strokeDash)
            };
        }

        public void ChangePenWidth(int witdh)
        {
            _penWidth = witdh;
        }

        public void ChangeStrokeDash(List<double> strokeDash)
        {
            _strokeDash = new List<double>(strokeDash);
        }

        public string ToJson()
        {
            Star2DData data = new Star2DData()
            {
                LeftTop = _leftTop.ToJson(),
                RightBottom = _rightBottom.ToJson(),
                PenWidth = _penWidth,
                StrokeDash = new List<double>(_strokeDash),
                Color = this.Color.ToString(),
                FillColor = this.FillColor.ToString()
            };
            return JsonSerializer.Serialize(data);
        }

        public IShape Parse(string json)
        {
            Star2DData data = (Star2DData)JsonSerializer.Deserialize(json, typeof(Star2DData));
            Color c = (Color)ColorConverter.ConvertFromString(data.Color);
            Color fc = (Color)ColorConverter.ConvertFromString(data.FillColor);
            Star2D result = new Star2D()
            {
                _leftTop = (Point2D)_leftTop.Parse(data.LeftTop),
                _rightBottom = (Point2D)_rightBottom.Parse(data.RightBottom),
                _penWidth = data.PenWidth,
                _strokeDash = new List<double>(data.StrokeDash),
                Color = new SolidColorBrush(c),
                FillColor = new SolidColorBrush(fc)
            };
            return result;
        }
    }
}
