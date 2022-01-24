using Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Paint
{
    public class Image2DData
    {
        public string LeftTop { get; set; }
        public string RightBottom { get; set; }
        public int PenWidth { get; set; }
        public List<double> StrokeDash { get; set; }
        public string Color { get; set; }
        public string FillColor { get; set; }

    }
    public class Image2D : IShape, INotifyPropertyChanged
    {
        private Point2D _leftTop = new Point2D();
        private Point2D _rightBottom = new Point2D();
        private int _penWidth = 1;
        private List<double> _strokeDash = new List<double>() { 0 };
        public event PropertyChangedEventHandler PropertyChanged;

        public SolidColorBrush Color { get; set; }
        public SolidColorBrush FillColor { get; set; }
        public ImageSource _source { get; set; }
        public string Name => "Image";

        public string Image { get; set; }

        
        public Image2D()
        {
            Color = new SolidColorBrush(Colors.Transparent);
            FillColor = new SolidColorBrush(Colors.Transparent);
            //Image = "/Ellipse2D;Component/images/ellipse.png";
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
            
            Image img = new Image()
            {
                Width = _source.Width,
                Height = _source.Height,
                Source = _source,
            };
            Canvas.SetLeft(img, left);
            Canvas.SetTop(img, top);
            return img;
        }

        public IShape NextShape()
        {
            //return new Ellipse2D();
            return null;
        }

        public IShape Clone()
        {
            return new Image2D()
            {
                _leftTop = (Point2D)this._leftTop.Clone(),
                _rightBottom = (Point2D)this._rightBottom.Clone(),
                _source = this._source,
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
            //Ellipse2DData data = new Ellipse2DData()
            //{
            //    LeftTop = _leftTop.ToJson(),
            //    RightBottom = _rightBottom.ToJson(),
            //    PenWidth = _penWidth,
            //    StrokeDash = new List<double>(_strokeDash),
            //    Color = this.Color.ToString(),
            //    FillColor = this.FillColor.ToString()
            //};
            //return JsonSerializer.Serialize(data);
            return null;
        }

        public IShape Parse(string json)
        {
            //Ellipse2DData data = (Ellipse2DData)JsonSerializer.Deserialize(json, typeof(Ellipse2DData));
            //Color c = (Color)ColorConverter.ConvertFromString(data.Color);
            //Color fc = (Color)ColorConverter.ConvertFromString(data.FillColor);
            //Ellipse2D result = new Ellipse2D()
            //{
            //    _leftTop = (Point2D)_leftTop.Parse(data.LeftTop),
            //    _rightBottom = (Point2D)_rightBottom.Parse(data.RightBottom),
            //    _penWidth = data.PenWidth,
            //    _strokeDash = new List<double>(data.StrokeDash),
            //    Color = new SolidColorBrush(c),
            //    FillColor = new SolidColorBrush(fc)
            //};
            //return result;
            return null;
        }
    }
}
