using Contract;
using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Text.Json;
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
        public string Source { get; set; }


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

        public byte[] getJPGFromImageControl(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.ToArray();
        }


        //public byte[] ImageSourceToBytes(ImageSource imageSource)
        //{
        //    byte[] bytes = null;
        //    var bitmapSource = imageSource as BitmapSource;

        //    if (bitmapSource != null)
        //    {
        //        var encoder = new JpegBitmapEncoder();
        //        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

        //        using (var stream = new MemoryStream())
        //        {
        //            encoder.Save(stream);
        //            bytes = stream.ToArray();
        //        }
        //    }

        //    return bytes;
        //}
        //public static ImageSource ByteToImage(byte[] imageData)
        //{
        //    BitmapImage biImg = new BitmapImage();
        //    MemoryStream ms = new MemoryStream(imageData);
        //    biImg.BeginInit();
        //    biImg.StreamSource = ms;
        //    biImg.EndInit();

        //    ImageSource imgSrc = biImg;

        //    return imgSrc;
        //}
        public string ToJson()
        {
            //var imageArray = ImageSourceToBytes(_source);

            //string imageString = Encoding.UTF8.GetString(imageArray);

            Image2DData data = new Image2DData()
            {
                LeftTop = _leftTop.ToJson(),
                RightBottom = _rightBottom.ToJson(),

                Source = _source.ToString(),
            };
            return JsonSerializer.Serialize(data);
        }
        
        public IShape Parse(string json)
        {
            Image2DData data = (Image2DData)JsonSerializer.Deserialize(json, typeof(Image2DData));
           
            var converter = new ImageSourceConverter();
            ImageSource imageBitmap = (ImageSource)converter.ConvertFromString(data.Source);
            Image2D result = new Image2D()
            {
                _leftTop = (Point2D)_leftTop.Parse(data.LeftTop),
                _rightBottom = (Point2D)_rightBottom.Parse(data.RightBottom),
                _source = imageBitmap,
            };
            return result;

        }
    }
}
