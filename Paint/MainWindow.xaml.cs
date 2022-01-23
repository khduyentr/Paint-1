using Contract;
using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public partial class MainWindow : Fluent.RibbonWindow, INotifyPropertyChanged
    {
        int totalShape = 0;
        BindingList<IShape> allShapes = new BindingList<IShape>();
        int selectedShape = -1;
        bool isDrawing = false;
        SolidColorBrush currentColor = new SolidColorBrush(Colors.Black);
        int currentPenWidthIndex = -1;
        int currentStrokeDashIndex = -1;
        IShape preview;
        BindingList<int> ComboboxPenWidth = new BindingList<int>();
        BindingList<List<double>> strokeDashArray = new BindingList<List<double>>();
        Project project = new Project();
        bool isSelectRegion = false;
        public void StartNewProject()
        {
            project = new Project();
            canvas.Children.Clear();
            Title = "Paint - " + project.GetName();
        }

       
        public MainWindow()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void CreateDLLFolder()
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string folderPath = System.IO.Path.GetDirectoryName(exePath);
            folderPath += @"\DLL";
            DirectoryInfo folder = new DirectoryInfo(folderPath);
            if (!folder.Exists)
            {
                Directory.CreateDirectory(folderPath);
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CreateDLLFolder();
            totalShape = ShapeFactory.GetInstance().ShapeAmount();
            for(int i = 0; i < totalShape; i++)
            {
                allShapes.Add(ShapeFactory.GetInstance().Create(i));
            }

            ShapeList.ItemsSource = allShapes;
            //Combobox for penwidth
            ComboboxPenWidth.Add(1);
            ComboboxPenWidth.Add(2);
            ComboboxPenWidth.Add(4);
            ComboboxPenWidth.Add(6);
            ComboboxPenWidth.Add(8);
            ComboboxPenWidth.Add(10);
            Pen_Width_Combo_Box.ItemsSource = ComboboxPenWidth;

            //Combobox for stroketype
            strokeDashArray.Add(new List<double>() { 1, 0 });
            strokeDashArray.Add(new List<double>() { 1 });
            strokeDashArray.Add(new List<double>() { 6, 2 });
            strokeDashArray.Add(new List<double>() { 3, 3, 1, 3 });
            strokeDashArray.Add(new List<double>() { 4, 1, 4 });
            StartNewProject();
            
            //Dash_Style_Combo_Box.ItemsSource = strokeDashArray;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = HandyControl.Controls.MessageBox.Show(new MessageBoxInfo
            {
                Message = "Open sth...",
                Caption = "Code open sth here",
                Button = MessageBoxButton.OK,
                IconBrushKey = ResourceToken.SuccessBrush,
                IconKey = ResourceToken.SuccessGeometry,
                StyleKey = "MessageBoxCustom"
            });
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedShape >= 0)
            {
                isDrawing = true;

                Point pos = e.GetPosition(canvas);

                preview.HandleStart(pos.X, pos.Y);
                preview.Color = currentColor;
                if (currentPenWidthIndex >= 0)
                {
                    preview.ChangePenWidth(ComboboxPenWidth[currentPenWidthIndex]);
                }
                else
                {
                    preview.ChangePenWidth(ComboboxPenWidth[0]);
                }

                if (currentStrokeDashIndex >= 0)
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
            if (selectedShape >= 0)
            {
                Canvas_Border.Cursor = Cursors.Cross;
            }
            if (isSelectRegion)
            {
                Canvas_Border.Cursor = Cursors.Cross;
            }
            if (isDrawing)
            {
                Point pos = e.GetPosition(canvas);
                preview.HandleEnd(pos.X, pos.Y);


                // Xoá hết các hình vẽ cũ
                canvas.Children.Clear();

                // Vẽ lại các hình trước đó
                foreach (var shape in project.UserShapes)
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

            if (selectedShape >= 0)
            {
                // Thêm đối tượng cuối cùng vào mảng quản lí
                Point pos = e.GetPosition(canvas);
                preview.HandleEnd(pos.X, pos.Y);
                project.UserShapes.Add(preview.Clone());
                project.IsSaved = false;
                Title = "Paint - " + project.GetName() + "*";

                // Sinh ra đối tượng mẫu kế
                preview = allShapes[selectedShape].NextShape();

                // Ve lai Xoa toan bo
                canvas.Children.Clear();

                // Ve lai tat ca cac hinh
                foreach (var shape in project.UserShapes)
                {
                    var element = shape.Draw();
                    canvas.Children.Add(element);
                }
            }
        }

        private void ShapeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedShape = ShapeList.SelectedIndex;
            if (selectedShape >= 0)
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
            picker.SelectedColorChanged += delegate { };
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
            if (!project.IsSaved)
            {
                MessageBoxResult msgResult = HandyControl.Controls.MessageBox.Show(new MessageBoxInfo
                {
                    Message = "Do you want to save changes to " + project.GetName(),
                    Caption = "Paint",
                    Button = MessageBoxButton.YesNoCancel,
                    IconBrushKey = ResourceToken.AccentBrush,
                    IconKey = ResourceToken.ErrorGeometry,
                    StyleKey = "MessageBoxCustom"
                });
                if (msgResult == MessageBoxResult.Yes)
                {
                    if (project.Address.Length == 0)
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.FileName = project.GetName();
                        saveFileDialog.DefaultExt = ".dat";
                        saveFileDialog.Filter = "DAT files(*.dat)|*.dat";
                        if (saveFileDialog.ShowDialog() == true)
                        {
                            string path = saveFileDialog.FileName;
                            project.Address = path;
                            project.SaveToFile();
                            StartNewProject();
                        }
                    }
                }
                else if (msgResult == MessageBoxResult.Cancel)
                {
                    return;
                }
                else
                {
                    StartNewProject();
                }
            }
            StartNewProject();
        }

        private void Save_File_Btn_Click(object sender, RoutedEventArgs e)
        {
           
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = project.GetName();
            saveFileDialog.DefaultExt = ".dat";
            saveFileDialog.Filter = "DAT files(*.dat)|*.dat";
            if (saveFileDialog.ShowDialog() == true)
            {
                string path = saveFileDialog.FileName;
                project.Address = path;
                project.SaveToFile();
                Title = "Paint - " + project.GetName();
            }
            
        }

        private void Open_File_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (!project.IsSaved)
            {
                MessageBoxResult msgResult = HandyControl.Controls.MessageBox.Show(new MessageBoxInfo
                {
                    Message = "Do you want to save changes to " + project.GetName(),
                    Caption = "Paint",
                    Button = MessageBoxButton.YesNoCancel,
                    IconBrushKey = ResourceToken.AccentBrush,
                    IconKey = ResourceToken.ErrorGeometry,
                    StyleKey = "MessageBoxCustom"
                });
                if(msgResult == MessageBoxResult.Yes)
                {
                    if (project.Address.Length == 0)
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.FileName = project.GetName();
                        saveFileDialog.DefaultExt = ".dat";
                        saveFileDialog.Filter = "DAT files(*.dat)|*.dat";
                        if (saveFileDialog.ShowDialog() == true)
                        {
                            string path = saveFileDialog.FileName;
                            project.Address = path;
                        }
                    }
                    project.SaveToFile();
                }
                else if(msgResult == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "DAT files only (*.dat)|*.dat";
            if (openFileDialog.ShowDialog() == true)
            {
                string path = openFileDialog.FileName;
                Project temProject = Project.Parse(path);
                if(temProject == null)
                {
                    HandyControl.Controls.MessageBox.Show(new MessageBoxInfo
                    {
                        Message = "Invalid file",
                        Caption = "Open File Error",
                        Button = MessageBoxButton.OK,
                        IconBrushKey = ResourceToken.AccentBrush,
                        IconKey = ResourceToken.ErrorGeometry,
                        StyleKey = "MessageBoxCustom"
                    });
                }
                else
                {
                    project = temProject.Clone();
                    Title = "Paint - " + project.GetName();

                    // Ve lai Xoa toan bo
                    canvas.Children.Clear();

                    // Ve lai tat ca cac hinh
                    foreach (var shape in project.UserShapes)
                    {
                        var element = shape.Draw();
                        canvas.Children.Add(element);
                    }
                }
            }
        }

        private void Save_As_Btn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = project.GetName();
            saveFileDialog.DefaultExt = ".dat";
            saveFileDialog.Filter = "DAT files(*.dat)|*.dat";
            if (saveFileDialog.ShowDialog() == true)
            {
                string path = saveFileDialog.FileName;
                project.Address = path;
                project.SaveToFile();
                Title = "Paint - " + project.GetName();
            }
        }

        private void Save_As_Bmp_Btn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = project.GetName();
            saveFileDialog.DefaultExt = ".bmp";
            saveFileDialog.Filter = "BMP files(*.bmp)|*.bmp";
            if (saveFileDialog.ShowDialog() == true)
            {
                Helper.SaveCanvasToFile((int)Canvas_Border.ActualWidth, (int)Canvas_Border.ActualHeight, canvas, saveFileDialog.FileName);
                HandyControl.Controls.MessageBox.Show(new MessageBoxInfo
                {
                    Message = "Export as BMP file successfully!",
                    Caption = "Paint",
                    Button = MessageBoxButton.OK,
                    IconBrushKey = ResourceToken.SuccessBrush,
                    IconKey = ResourceToken.SuccessGeometry,
                    StyleKey = "MessageBoxCustom"
                });
            }
        }

        private void Save_As_Jpg_Btn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = project.GetName();
            saveFileDialog.DefaultExt = ".jpg";
            saveFileDialog.Filter = "JPG files(*.jpg)|*.jpg";
            if (saveFileDialog.ShowDialog() == true)
            {
                Helper.SaveCanvasToFile((int)Canvas_Border.ActualWidth, (int)Canvas_Border.ActualHeight, canvas, saveFileDialog.FileName);
                HandyControl.Controls.MessageBox.Show(new MessageBoxInfo
                {
                    Message = "Export as JPG file successfully!",
                    Caption = "Paint",
                    Button = MessageBoxButton.OK,
                    IconBrushKey = ResourceToken.SuccessBrush,
                    IconKey = ResourceToken.SuccessGeometry,
                    StyleKey = "MessageBoxCustom"
                });
            }
        }

        private void Save_As_Png_Btn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = project.GetName();
            saveFileDialog.DefaultExt = ".png";
            saveFileDialog.Filter = "PNG files(*.png)|*.png";
            if (saveFileDialog.ShowDialog() == true)
            {
                Helper.SaveCanvasToFile((int)Canvas_Border.ActualWidth, (int)Canvas_Border.ActualHeight, canvas, saveFileDialog.FileName);
                HandyControl.Controls.MessageBox.Show(new MessageBoxInfo
                {
                    Message = "Export as PNG file successfully!",
                    Caption = "Paint",
                    Button = MessageBoxButton.OK,
                    IconBrushKey = ResourceToken.SuccessBrush,
                    IconKey = ResourceToken.SuccessGeometry,
                    StyleKey = "MessageBoxCustom"
                });
            }
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
            isSelectRegion = !isSelectRegion;
            ShapeList.SelectedIndex = -1;
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

        private void RibbonWindow_SizeChanged(object sender, SizeChangedEventArgs e)
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

        private void RibbonWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!project.IsSaved)
            {
                MessageBoxResult msgResult = HandyControl.Controls.MessageBox.Show(new MessageBoxInfo
                {
                    Message = "Do you want to save changes to " + project.GetName(),
                    Caption = "Paint",
                    Button = MessageBoxButton.YesNoCancel,
                    IconBrushKey = ResourceToken.AccentBrush,
                    IconKey = ResourceToken.ErrorGeometry,
                    StyleKey = "MessageBoxCustom"
                });
                if (msgResult == MessageBoxResult.Yes)
                {
                    if (project.Address.Length == 0)
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        saveFileDialog.FileName = project.GetName();
                        saveFileDialog.DefaultExt = ".dat";
                        saveFileDialog.Filter = "DAT files(*.dat)|*.dat";
                        if (saveFileDialog.ShowDialog() == true)
                        {
                            string path = saveFileDialog.FileName;
                            project.Address = path;
                            project.SaveToFile();
                        }
                    }
                }
                else if (msgResult == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
        }
    }
}
