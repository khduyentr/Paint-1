using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Contract
{
    public interface IShape
    {
        public SolidColorBrush Color { get; set; }
        public string Image { get; set; }
        string Name { get; }
        public void HandleStart(double x, double y);
        public void HandleEnd(double x, double y);

        public UIElement Draw();
        public IShape NextShape();

        public IShape Clone();

        public void ChangePenWidth(int witdh);

        public void ChangeStrokeDash(List<double> strokeDash);

        public string ToJson();

        public IShape Parse(string json);

    }
}
