using Contract;
using HandyControl.Controls;
using HandyControl.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window, INotifyPropertyChanged
    {
        int totalShape = 0;
        BindingList<IShape> allShapes = new BindingList<IShape>();
        int selectedShape = -1;
        bool isDrawing = false;
        SolidColorBrush currentColor = new SolidColorBrush(Colors.Black);
        int currentPenWidthIndex = -1;
        int currentStrokeDashIndex = -1;
        IShape preview;
        List<IShape> userShapes = new List<IShape>();
        BindingList<int> ComboboxPenWidth = new BindingList<int>();
        BindingList<List<double>> strokeDashArray = new BindingList<List<double>>();
        public MainWindow()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            totalShape = ShapeFactory.GetInstance().ShapeAmount();
            for(int i = 0; i < totalShape; i++)
            {
                allShapes.Add(ShapeFactory.GetInstance().Create(i));
            }

            //Combobox for penwidth
            
            ShapeList.ItemsSource = allShapes;
            ComboboxPenWidth.Add(1);
            ComboboxPenWidth.Add(2);
            ComboboxPenWidth.Add(4);
            ComboboxPenWidth.Add(6);
            ComboboxPenWidth.Add(8);
            ComboboxPenWidth.Add(10);
            strokeDashArray.Add(new List<double>() { 1,0 });
            strokeDashArray.Add(new List<double>() { 1 });
            strokeDashArray.Add(new List<double>() { 6,2});
            strokeDashArray.Add(new List<double>() { 3,3,1,3});
            strokeDashArray.Add(new List<double>() { 4,1,4 });
            Pen_Width_Combo_Box.ItemsSource = ComboboxPenWidth;
            //Dash_Style_Combo_Box.ItemsSource = strokeDashArray;

        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(selectedShape >= 0)
            {
                isDrawing = true;

                Point pos = e.GetPosition(canvas);

                preview.HandleStart(pos.X, pos.Y);
                preview.Color = currentColor;
                if(currentPenWidthIndex >= 0)
                {
                    preview.ChangePenWidth(ComboboxPenWidth[currentPenWidthIndex]);
                }
                else
                {
                    preview.ChangePenWidth(ComboboxPenWidth[0]);
                }
               
                if(currentStrokeDashIndex >= 0)
                {
                    preview.ChangeStrokeDash(strokeDashArray[currentStrokeDashIndex]);
                }
                else
                {
                    preview.ChangeStrokeDash(strokeDashArray[0]);
                }
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                Point pos = e.GetPosition(canvas);
                preview.HandleEnd(pos.X, pos.Y);
                

                // Xoá hết các hình vẽ cũ
                canvas.Children.Clear();

                // Vẽ lại các hình trước đó
                foreach (var shape in userShapes)
                {
                    UIElement element = shape.Draw();
                    canvas.Children.Add(element);
                }

                // Vẽ hình preview đè lên
                canvas.Children.Add(preview.Draw());
            }
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;

            if(selectedShape >= 0)
            {
                // Thêm đối tượng cuối cùng vào mảng quản lí
                Point pos = e.GetPosition(canvas);
                preview.HandleEnd(pos.X, pos.Y);
                userShapes.Add(preview.Clone());

                // Sinh ra đối tượng mẫu kế
                preview = allShapes[selectedShape].NextShape();

                // Ve lai Xoa toan bo
                canvas.Children.Clear();

                // Ve lai tat ca cac hinh
                foreach (var shape in userShapes)
                {
                    var element = shape.Draw();
                    canvas.Children.Add(element);
                }
            }
        }

        private void ShapeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedShape = ShapeList.SelectedIndex;
            if(selectedShape >= 0)
            {
                preview = allShapes[selectedShape].Clone();
            }
           
        }

        private void Open_ColorPicker_Click(object sender, RoutedEventArgs e)
        {
            var picker = SingleOpenHelper.CreateControl<ColorPicker>();
            var window = new PopupWindow
            {
                PopupElement = picker
            };
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            picker.SelectedColorChanged += delegate {};
            picker.Confirmed += delegate
            {
                currentColor = picker.SelectedBrush;
                Color_Preview.Fill = currentColor;
                window.Close();
            };
            picker.Canceled += delegate { window.Close(); };
            window.Show(Open_ColorPicker, false);
        }

        private void New_File_Btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Save_File_Btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Open_File_Btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Save_As_Btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cut_Btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Copy_Btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Paste_Btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Undo_Btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Redo_Btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Select_Area_Btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Crop_Area_Btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Resize_Area_Btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Rotate_Area_Btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Pen_Width_Combo_Box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentPenWidthIndex = Pen_Width_Combo_Box.SelectedIndex;
        }

        private void Dash_Style_Combo_Box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentStrokeDashIndex = Dash_Style_Combo_Box.SelectedIndex;
        }
    }
}
