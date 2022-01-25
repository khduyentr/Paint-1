using Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Paint
{
    public class BrushStrokeData
    {
        public string LeftTop { get; set; }
        public string RightBottom { get; set; }
        public int PenWidth { get; set; }
        public List<string> Points { get; set; }
        public List<double> StrokeDash { get; set; }
        public string FillColor { get; set; }
        public string Color { get; set; }
    }
    class BrushStroke : IShape, INotifyPropertyChanged
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();

        private List<Point2D> points = new List<Point2D>();
        private int _penWidth = 1;
        private List<double> _strokeDash = new List<double>() { 0 };
        public event PropertyChangedEventHandler PropertyChanged;

        public SolidColorBrush FillColor { get; set; }
        public SolidColorBrush Color { get; set; }
        public string Name => "Brush Stroke";

        public string Image { get; set; }
        public BrushStroke()
        {
            Color = new SolidColorBrush(Colors.Black);
            FillColor = new SolidColorBrush(Colors.Transparent);
            Image = "/Paint;Component/images/brush.png";
        }
        public void HandleStart(double x, double y)
        {
            _leftTop = new Point2D() { X = x, Y = y };
            points.Add(new Point2D() { X = x, Y = y });
        }

        public void HandleEnd(double x, double y)
        {
            _rightBottom = new Point2D() { X = x, Y = y };
            points.Add(new Point2D() { X = x, Y = y });
        }

        public UIElement Draw()
        {
            double left = (_rightBottom.X > _leftTop.X) ? _leftTop.X : _rightBottom.X;
            double top = (_rightBottom.Y > _leftTop.Y) ? _leftTop.Y : _rightBottom.Y;
          
            PathFigure myPathFigure = new PathFigure();

            myPathFigure.StartPoint = new Point(_leftTop.X, _leftTop.Y);

            PointCollection myPointCollection = new PointCollection();
            foreach (var p in points)
            {
                myPointCollection.Add(new Point(p.X, p.Y));
            }

            PolyBezierSegment myBezierSegment = new PolyBezierSegment();
            myBezierSegment.Points = myPointCollection;

            PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();
            myPathSegmentCollection.Add(myBezierSegment);

            myPathFigure.Segments = myPathSegmentCollection;

            PathFigureCollection myPathFigureCollection = new PathFigureCollection();
            myPathFigureCollection.Add(myPathFigure);

            PathGeometry myPathGeometry = new PathGeometry();
            myPathGeometry.Figures = myPathFigureCollection;

            Path myPath = new Path();
            myPath.Stroke = Color;
            myPath.StrokeThickness = _penWidth;
            myPath.StrokeDashArray = new DoubleCollection(_strokeDash);
            myPath.Data = myPathGeometry;
            return myPath;
        }

        public IShape NextShape()
        {
            return new BrushStroke();
        }

        public IShape Clone()
        {
            return new BrushStroke()
            {
                _leftTop = (Point2D)this._leftTop.Clone(),
                _rightBottom = (Point2D)this._rightBottom.Clone(),
                Color = this.Color,
                points = new List<Point2D>(this.points),
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
            List<string> dataPoints = new List<string>();
            foreach(var p in points)
            {
                dataPoints.Add(p.ToJson());
            }
            BrushStrokeData data = new BrushStrokeData()
            {
                LeftTop = _leftTop.ToJson(),
                RightBottom = _rightBottom.ToJson(),
                PenWidth = _penWidth,
                Points = new List<string>(dataPoints),
                StrokeDash = new List<double>(_strokeDash),
                Color = this.Color.ToString(),
                FillColor = this.FillColor.ToString()
            };
            return JsonSerializer.Serialize(data);
        }

        public IShape Parse(string json)
        {
            BrushStrokeData data = (BrushStrokeData)JsonSerializer.Deserialize(json, typeof(BrushStrokeData));
            Color c = (Color)ColorConverter.ConvertFromString(data.Color);
            Color fc = (Color)ColorConverter.ConvertFromString(data.FillColor);
            List<Point2D> pointData = new List<Point2D>();
            foreach(var p in data.Points)
            {
                pointData.Add((Point2D)_leftTop.Parse(p));
            }
            BrushStroke result = new BrushStroke()
            {
                _leftTop = (Point2D)_leftTop.Parse(data.LeftTop),
                _rightBottom = (Point2D)_rightBottom.Parse(data.RightBottom),
                _penWidth = data.PenWidth,
                points = new List<Point2D>(pointData),
                _strokeDash = new List<double>(data.StrokeDash),
                Color = new SolidColorBrush(c),
                FillColor = new SolidColorBrush(fc)
            };
            return result;
        }
    }
}
