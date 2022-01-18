using Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Circle2D
{
    public class Circle2D : IShape, INotifyPropertyChanged
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();
        private int _penWidth = 1;
        private List<double> _strokeDash = new List<double>() { 0 };
        public event PropertyChangedEventHandler PropertyChanged;

        public SolidColorBrush Color { get; set; }
        public string Name => "Circle";

        public string Image { get; set; }
        public Circle2D()
        {
            Color = new SolidColorBrush(Colors.Black);
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
    }
}
