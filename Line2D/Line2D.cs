using Contract;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Line2D
{
    public class Line2D : IShape, INotifyPropertyChanged
    {
        private Point2D _start = new Point2D();
        private Point2D _end = new Point2D();
        private int _penWidth = 1;
        public event PropertyChangedEventHandler PropertyChanged;

        public SolidColorBrush Color { get; set; }
        public string Name => "Line";

        public string Image { get; set; }
        public Line2D()
        {
            Color = new SolidColorBrush(Colors.Black);
            Image = "/Line2D;Component/images/line.png";
        }
        public void HandleStart(double x, double y)
        {
            _start = new Point2D() { X = x, Y = y };
        }

        public void HandleEnd(double x, double y)
        {
            _end = new Point2D() { X = x, Y = y };
        }

        public UIElement Draw()
        {
            Line l = new Line()
            {
                X1 = _start.X,
                Y1 = _start.Y,
                X2 = _end.X,
                Y2 = _end.Y,
                StrokeDashArray = {1,6},
                StrokeThickness = _penWidth,
                Stroke = Color,
            };

            return l;
        }

        public IShape NextShape()
        {
            return new Line2D();
        }

        public IShape Clone()
        {
            return new Line2D() {
                _start = (Point2D)this._start.Clone(),
                _end = (Point2D)this._end.Clone(),
                Color = this.Color,
                _penWidth = this._penWidth
            };
        }

        public void ChangePenWidth(int witdh)
        {
            _penWidth = witdh;
        }
    }
}
