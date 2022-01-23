using Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Circle2D
{
    public class Circle2DData
    {
        public string LeftTop { get; set; }
        public string RightBottom { get; set; }
        public int PenWidth { get; set; }
        public List<double> StrokeDash { get; set; }
        public string FillColor { get; set; }
        public string Color { get; set; }
    }
    public class Circle2D : IShape, INotifyPropertyChanged
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();
        private int _penWidth = 1;
        private List<double> _strokeDash = new List<double>() { 0 };
        public event PropertyChangedEventHandler PropertyChanged;

        public SolidColorBrush FillColor { get; set; }
        public SolidColorBrush Color { get; set; }
        public string Name => "Circle";

        public string Image { get; set; }
        public Circle2D()
        {
            Color = new SolidColorBrush(Colors.Black);
            FillColor = new SolidColorBrush(Colors.Transparent);
            Image = "/Circle2D;Component/images/circle.png";
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
            double size = (Math.Abs(_rightBottom.X - _leftTop.X) > Math.Abs(_rightBottom.Y - _leftTop.Y)) ? Math.Abs(_rightBottom.Y - _leftTop.Y) : Math.Abs(_rightBottom.X - _leftTop.X);

            double left = _leftTop.X;
            if (_rightBottom.X < _leftTop.X)
            {
                left -= size;
            }
            double top = _leftTop.Y;
            if (_rightBottom.Y < _leftTop.Y)
            {
                top -= size;
            }
            Ellipse e = new Ellipse()
            {
                Width = size,
                Height = size,
                StrokeDashArray = new DoubleCollection(_strokeDash),
                StrokeThickness = _penWidth,
                Stroke = Color,
                Fill = FillColor
            };
            Canvas.SetLeft(e, left);
            Canvas.SetTop(e, top);
            return e;
        }

        public IShape NextShape()
        {
            return new Circle2D();
        }

        public IShape Clone()
        {
            return new Circle2D()
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
            Circle2DData data = new Circle2DData()
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
            Circle2DData data = (Circle2DData)JsonSerializer.Deserialize(json, typeof(Circle2DData));
            Color c = (Color)ColorConverter.ConvertFromString(data.Color);
            Color fc = (Color)ColorConverter.ConvertFromString(data.FillColor);
            Circle2D result = new Circle2D()
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
