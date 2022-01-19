using Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Line2D
{
    public class Line2DData
    {
        public string Start {get;set;}
        public string End { get; set; }
        public int PenWidth { get; set; }
        public List<double> StrokeDash { get; set; }
        public string Color { get; set; }
    }
    public class Line2D : IShape, INotifyPropertyChanged
    {
        private Point2D _start = new Point2D();
        private Point2D _end = new Point2D();
        private int _penWidth = 1;
        private List<double> _strokeDash = new List<double>() { 0};
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
                StrokeDashArray = new DoubleCollection(_strokeDash),
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
            Line2DData data = new Line2DData()
            {
                Start = _start.ToJson(),
                End = _end.ToJson(),
                PenWidth = _penWidth,
                StrokeDash = new List<double>(_strokeDash),
                Color = this.Color.ToString()
            };
            return JsonSerializer.Serialize(data);
        }

        public IShape Parse(string json)
        {
            Line2DData data = (Line2DData)JsonSerializer.Deserialize(json, typeof(Line2DData));
            Color c = (Color)ColorConverter.ConvertFromString(data.Color);
            Line2D result = new Line2D()
            {
                _start = (Point2D)_start.Parse(data.Start),
                _end = (Point2D)_start.Parse(data.End),
                _penWidth = data.PenWidth,
                _strokeDash = new List<double>(data.StrokeDash),
                Color = new SolidColorBrush(c)
            };
            return result;
        }
    }
}
