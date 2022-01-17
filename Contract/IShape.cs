using System;
using System.Windows;
using System.Windows.Media;

namespace Contract
{
    public interface IShape
    {
        public SolidColorBrush Color { get; set; }
        public string Image { get; set; }
        string Name { get; }
        void HandleStart(double x, double y);
        void HandleEnd(double x, double y);

        UIElement Draw();
        IShape NextShape();

        IShape Clone();

        void ChangePenWidth(int witdh);

    }
}
