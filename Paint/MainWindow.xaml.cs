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
        int currentPenWidth = 1;
        IShape preview;
        List<IShape> userShapes = new List<IShape>();
        BindingList<int> ComboboxPenWidth = new BindingList<int>();
       
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
                for(int j = 0; j < 15; j++)
                {
                    allShapes.Add(ShapeFactory.GetInstance().Create(i));
                }
            }

            //Combobox for penwidth
            for(int i = 0; i < 10; i++)
            {
                ComboboxPenWidth.Add(i + 1);
            }
            Pen_Width_Combo_Box.ItemsSource = ComboboxPenWidth;
            ShapeList.ItemsSource = allShapes;
            
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(selectedShape >= 0)
            {
                isDrawing = true;

                Point pos = e.GetPosition(canvas);

                preview.HandleStart(pos.X, pos.Y);
                preview.Color = currentColor;
                preview.ChangePenWidth(currentPenWidth);
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
            currentPenWidth = (int)Pen_Width_Combo_Box.SelectedItem;
        }
    }
}
