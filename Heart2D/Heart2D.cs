using Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Heart2D
{
    public class Heart2DData
    {
        public string LeftTop { get; set; }
        public string RightBottom { get; set; }
        public int PenWidth { get; set; }
        public List<double> StrokeDash { get; set; }
        public string Color { get; set; }
        public string FillColor { get; set; }
    }
    public class Heart2D : IShape, INotifyPropertyChanged
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();
        private int _penWidth = 1;
        private List<double> _strokeDash = new List<double>() { 0 };
        public SolidColorBrush Color { get; set; }
        public string Name => "Heart";
        public SolidColorBrush FillColor { get; set; }
        public string Image { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Heart2D()
        {
            Color = new SolidColorBrush(Colors.Black);
            FillColor = new SolidColorBrush(Colors.Transparent);
            Image = "/Heart2D;Component/images/heart.png";
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
            var converter = TypeDescriptor.GetConverter(typeof(Geometry));
            string pathData = "M 90,50 A 20,20 0 0 0 50,90 C 60,100 90,120 90,120 C 90,120 110,110 130,90A 20,20 0 0 0 90,50";
            var path = new Path
            {
                Stretch = Stretch.Fill,
                Data = (Geometry)converter.ConvertFrom(pathData),
                Width = w,
                Height = h,
                Fill = FillColor
            };
            path.Stroke = Color;
            path.StrokeThickness = _penWidth;
            path.StrokeDashArray = new DoubleCollection(_strokeDash);
            Canvas.SetLeft(path, left);
            Canvas.SetTop(path, top);
            return path;
        }

        public IShape NextShape()
        {
            return new Heart2D();
        }

        public IShape Clone()
        {
            return new Heart2D()
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
            Heart2DData data = new Heart2DData()
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
            Heart2DData data = (Heart2DData)JsonSerializer.Deserialize(json, typeof(Heart2DData));
            Color c = (Color)ColorConverter.ConvertFromString(data.Color);
            Color fc = (Color)ColorConverter.ConvertFromString(data.FillColor);
            Heart2D result = new Heart2D()
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
