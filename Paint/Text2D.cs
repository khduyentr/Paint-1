using Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Paint
{
    public class Text2DData
    {
        public string Content { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public bool IsUnderline { get; set; }
        public bool IsStrike { get; set; }
        public string LeftTop { get; set; }
        public int PenWidth { get; set; }
        public List<double> StrokeDash { get; set; }
        public string Color { get; set; }
        public string FillColor { get; set; }
    }
    public class Text2D : IShape, INotifyPropertyChanged
    {
        private string _content = "";
        private FontFamily _fontFamily = new FontFamily("Arial");
        private int _fontSize = 10;
        private bool _isBold = false;
        private bool _isItalic = false;
        private bool _isUnderline = false;
        private bool _isStrike = false;
        private Point2D _leftTop = new Point2D();
        private int _penWidth = 1;
        private List<double> _strokeDash = new List<double>() { 0 };

        //foreground
        public SolidColorBrush Color { get; set; }
        //background
        public SolidColorBrush FillColor { get; set; }
        public string Image { get; set; }

        public string Name => "Text";

        public event PropertyChangedEventHandler PropertyChanged;

        public Text2D()
        {
            Color = new SolidColorBrush(Colors.Black);
            FillColor = new SolidColorBrush(Colors.Transparent);
            Image = "/Text2D;Component/images/text.png";
        }

        public void setContent(string content)
        {
            _content = content;
        }

        public void setFontFamily(FontFamily font)
        {
            _fontFamily = font;
        }

        public void setFontSize(int size)
        {
            _fontSize = size;
        }

        public void setBold(bool bold)
        {
            _isBold = bold;
        }

        public void setItalic(bool italic)
        {
            _isItalic = italic;
        }

        public void setUnderline(bool underline)
        {
            _isUnderline = underline;
        }

        public void setStrike(bool strike)
        {
            _isStrike = strike;
        }

        public void ChangePenWidth(int witdh)
        {
            _penWidth = witdh;
        }

        public void ChangeStrokeDash(List<double> strokeDash)
        {
            _strokeDash = new List<double>(strokeDash);
        }

        public IShape Clone()
        {
            return new Text2D()
            {
                _content = this._content,
                _fontFamily = this._fontFamily,
                _fontSize = this._fontSize,
                _isBold = this._isBold,
                _isItalic = this._isItalic,
                _isUnderline = this._isUnderline,
                _isStrike = this._isStrike,
                _leftTop = (Point2D)this._leftTop.Clone(),
                Color = this.Color,
                FillColor = this.FillColor,
                _penWidth = this._penWidth,
                _strokeDash = new List<double>(this._strokeDash)
            };
        }

        public UIElement Draw()
        {
            TextBlock text = new TextBlock()
            {
                Text = _content,
                Foreground = Color,
                Background = FillColor,
                FontFamily = _fontFamily,
                FontSize = _fontSize,
                FontWeight = (_isBold) ? FontWeights.Bold : FontWeights.Normal,
                FontStyle = (_isItalic) ? FontStyles.Italic : FontStyles.Normal
            };
            if (_isUnderline) text.TextDecorations = TextDecorations.Underline;
            if (_isStrike) text.TextDecorations = TextDecorations.Strikethrough;

            Canvas.SetLeft(text, _leftTop.X);
            Canvas.SetTop(text, _leftTop.Y);
            return text;
        }

        public void HandleEnd(double x, double y)
        {
            _leftTop = new Point2D() { X = x, Y = y };
        }

        public void HandleStart(double x, double y)
        {
            _leftTop = new Point2D() { X = x, Y = y };
        }

        public IShape NextShape()
        {
            return new Text2D();
        }

        public IShape Parse(string json)
        {
            Text2DData data = (Text2DData)JsonSerializer.Deserialize(json, typeof(Text2DData));
            Color c = (Color)ColorConverter.ConvertFromString(data.Color);
            Color fc = (Color)ColorConverter.ConvertFromString(data.FillColor);
            Text2D result = new Text2D()
            {
                _content = data.Content,
                _fontFamily = new FontFamily(data.FontFamily),
                _fontSize = data.FontSize,
                _isBold = data.IsBold,
                _isItalic = data.IsItalic,
                _isUnderline = data.IsUnderline,
                _isStrike = data.IsStrike,
                _leftTop = (Point2D)_leftTop.Parse(data.LeftTop),
                _penWidth = data.PenWidth,
                _strokeDash = new List<double>(data.StrokeDash),
                Color = new SolidColorBrush(c),
                FillColor = new SolidColorBrush(fc)
            };
            return result;
        }

        public string ToJson()
        {
            Text2DData data = new Text2DData()
            {
                Content = _content,
                FontFamily = this._fontFamily.ToString(),
                FontSize = this._fontSize,
                IsBold = this._isBold,
                IsItalic = this._isItalic,
                IsUnderline = this._isUnderline,
                IsStrike = this._isStrike,
                LeftTop = _leftTop.ToJson(),
                PenWidth = _penWidth,
                StrokeDash = new List<double>(_strokeDash),
                Color = this.Color.ToString(),
                FillColor = this.FillColor.ToString()
            };
            return JsonSerializer.Serialize(data);
        }
    }
}
