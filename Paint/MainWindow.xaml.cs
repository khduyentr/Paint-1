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
    
    public class layerView: INotifyPropertyChanged
    {
        public layerView()
        {
        }

        public layerView(string name, bool isVisible)
        {
            this.name = name;
            this.isVisible = isVisible;
        }

        public string name { get; set; }
        public bool isVisible { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public partial class MainWindow : Fluent.RibbonWindow, INotifyPropertyChanged
    {
        // project management
        Project project = new Project();
        List<Project> list_project = new List<Project>();
        List<Project> undo_project = new List<Project>();

        //layer management
        int totalLayer = 0;
        int selectedLayer = -1;
        const int maxLayerAmount = 10000 ;
        
        BindingList<layerView> allLayers = new BindingList<layerView>();

        // shape management
        int totalShape = 0;
        int selectedShape = -1;
        bool isBrushStroke = false;
        bool isAddText = false;
        IShape preview;
        BindingList<IShape> allShapes = new BindingList<IShape>();
        bool isPreview = false;

        // penwidth management
        int currentPenWidthIndex = -1;
        int currentStrokeDashIndex = -1;
        ScaleTransform st = new ScaleTransform(); 
        BindingList<int> ComboboxPenWidth = new BindingList<int>();
        BindingList<List<double>> strokeDashArray = new BindingList<List<double>>();

        // color brush
        SolidColorBrush currentColor = new SolidColorBrush(Colors.Black);
        SolidColorBrush currentFillColor = new SolidColorBrush(Colors.Transparent);

        //font
        BindingList<FontFamily> FontList = new BindingList<FontFamily>();
        BindingList<int> FontSizeList = new BindingList<int>();
        FontFamily currentFontFamily = new FontFamily("Arial");
        int currentFontSize = 10;
        SolidColorBrush currentTextForeground = new SolidColorBrush(Colors.Black);
        SolidColorBrush currentTextBackground = new SolidColorBrush(Colors.Transparent);
        bool isBold = false;
        bool isItalic = false;
        bool isUnderline = false;
        bool isStrike = false;

        // drawing variable
        bool isDrawing = false;
        bool isSelectRegion = false;

        List<IShape> undo = new List<IShape>();
        public double ZoomValue { get; set; }

        public void StartNewProject()
        {
            Undo_Btn.IsEnabled = false;
            Redo_Btn.IsEnabled = false;
            undo_project.Clear();
            project = new Project();
            list_project.Add(project.Clone());
            canvas.Children.Clear();
            allLayers.Clear();
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
            ZoomValue = 100;
            totalShape = ShapeFactory.GetInstance().ShapeAmount();
            for(int i = 0; i < totalShape; i++)
            {
                allShapes.Add(ShapeFactory.GetInstance().Create(i));
            }
            ShapeList.ItemsSource = allShapes;
            LayerList.ItemsSource = allLayers;

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
            Canvas_Container.LayoutTransform = st;
            
            DataContext = this;
            Zoom_Slider.Value = 100;
            //Dash_Style_Combo_Box.ItemsSource = strokeDashArray;

            //Font
            var fonts = Helper.GetAllFonts();
            foreach (var font in fonts)
            {
                FontList.Add(font);
            }
            Font_Combo_Box.ItemsSource = FontList;
            Font_Combo_Box.SelectedItem = currentFontFamily;

            FontSizeList.Add(8);
            FontSizeList.Add(9);
            FontSizeList.Add(10);
            FontSizeList.Add(11);
            FontSizeList.Add(12);
            FontSizeList.Add(14);
            FontSizeList.Add(16);
            FontSizeList.Add(18);
            FontSizeList.Add(20);
            FontSizeList.Add(22);
            FontSizeList.Add(24);
            FontSizeList.Add(26);
            FontSizeList.Add(28);
            FontSizeList.Add(36);
            FontSizeList.Add(48);
            FontSizeList.Add(72);
            Font_Size_Combo_Box.ItemsSource = FontSizeList;
            Font_Size_Combo_Box.SelectedItem = currentFontSize;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        Point textPoint, lastTextPoint;
        bool canfocus = false;

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedShape >= 0 || isBrushStroke)
            {
                isDrawing = true;

                Point pos = e.GetPosition(canvas);

                preview.HandleStart(pos.X, pos.Y);
                preview.Color = currentColor;
                preview.FillColor = currentFillColor;
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
            if (isAddText)
            {
                var input = new System.Windows.Controls.TextBox()
                {
                    FontFamily = currentFontFamily,
                    FontSize = currentFontSize,
                    Foreground = currentTextForeground,
                    Background = currentTextBackground,
                    FontWeight = (isBold) ? FontWeights.Bold : FontWeights.Normal,
                    FontStyle = (isItalic) ? FontStyles.Italic : FontStyles.Normal
                };
                if (isUnderline) input.TextDecorations = TextDecorations.Underline;
                if (isStrike) input.TextDecorations = TextDecorations.Strikethrough;
                canfocus = true;
                lastTextPoint = textPoint;
                textPoint = e.GetPosition(canvas);
                Canvas.SetLeft(input, textPoint.X - 10);
                Canvas.SetTop(input, textPoint.Y - 10);
                canvas.Children.Add(input);
                input.Focus();
                input.LostFocus += Input_LostFocus;
            }
        }

        //tương tự mousemove + mouseup nhưng là cho text (sửa layer, undo, redo thì nhớ sửa ở đây nựa)
        private void Input_LostFocus(object sender, RoutedEventArgs e)
        {
            var input = (System.Windows.Controls.TextBox)sender;
            if (input.Text != "")
            {
                var newText = new Text2D();
                newText.setContent(input.Text);
                newText.Color = currentTextForeground;
                newText.FillColor = currentTextBackground;
                newText.setFontFamily(currentFontFamily);
                newText.setFontSize(currentFontSize);
                newText.setBold(isBold);
                newText.setItalic(isItalic);
                newText.setUnderline(isUnderline);
                newText.setStrike(isStrike);
                if (!canfocus) lastTextPoint = textPoint; 
                newText.HandleStart(lastTextPoint.X, lastTextPoint.Y);

                //tương tự mousemove
                //if (selectedLayer >= 0)
                //{
                //    // Vẽ lại các hình trước đó
                //    for (int i = 0; i < project.UserLayer.Count; i++)
                //    {
                //        if (project.UserLayer[i].isVisible)
                //        {
                //            if (selectedLayer == i)
                //            {
                //                canvas.Children.Add(newText.Draw());
                //            }

                //        }
                //    }
                //}
                //else
                //{
                //    canvas.Children.Add(newText.Draw());
                //}

                // tương tự mouseup
                if (selectedLayer >= 0)
                {
                    // Thêm đối tượng cuối cùng vào mảng quản lí
                    project.UserLayer[selectedLayer].UserShapes.Add(newText.Clone());
                    project.IsSaved = false;
                    list_project.Add(project.Clone());

                    Title = "Paint - " + project.GetName() + "*";
                }
                else
                {
                    // Thêm đối tượng cuối cùng vào mảng quản lí
                    allLayers.Insert(0, new layerView(project.addNewLayer(), true));
                    project.UserLayer[project.UserLayer.Count() - 1].UserShapes.Add(newText.Clone());
                    project.IsSaved = false;
                    list_project.Add(project.Clone());

                    Title = "Paint - " + project.GetName() + "*";
                }
                canvas.Children.Add(newText.Draw());
                Undo_Btn.IsEnabled = true;
                Redo_Btn.IsEnabled = false;
                undo_project.Clear();
            }

            canvas.Children.Remove(input);
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (LayerList.SelectedIndex >= 0)
            {
                selectedLayer = allLayers.Count - 1 - LayerList.SelectedIndex;
            }
            else
            {
                selectedLayer = -1;
            }

            if (selectedShape >= 0)
            {
                Canvas_Border.Cursor = Cursors.Cross;
            }
            if (isSelectRegion)
            {
                Canvas_Border.Cursor = Cursors.Cross;
            }
            if (isBrushStroke)
            {
                Canvas_Border.Cursor = Cursors.Pen;
            }
            if (isAddText)
            {
                Canvas_Border.Cursor = Cursors.IBeam;
            }
            if (isDrawing)
            {
                Point pos = e.GetPosition(canvas);
                preview.HandleEnd(pos.X, pos.Y);



                if (selectedLayer >= 0)
                {

                    int previewPos = 0;
                    for (int i = 0; i < project.UserLayer.Count; i++)
                    {

                        if (project.UserLayer[i].isVisible)
                        {
                            previewPos += project.UserLayer[i].UserShapes.Count;
                            if (selectedLayer == i)
                            {
                                if (isPreview)
                                {
                                    canvas.Children.RemoveAt(previewPos);
                                }
                                isPreview = true;
                                canvas.Children.Insert(previewPos, preview.Draw());
                               
                                break;
                            }
                            
                           
                        }
                    }
 
                }
                else
                {
                    // Xoá hết các hình vẽ cũ - OLD
                    if (isPreview)
                    {
                        canvas.Children.RemoveAt(canvas.Children.Count - 1);
                    }

                    // Vẽ hình preview đè lên
                    isPreview = true;
                    canvas.Children.Add(preview.Draw());

                }
                
            }
        }

        private void reDraw()
        {
            // Ve lai Xoa toan bo
            canvas.Children.Clear();

            // Ve lai tat ca cac hinh

            foreach (var layer in project.UserLayer)
            {
                if (layer.isVisible)
                {
                    foreach (var shape in layer.UserShapes)
                    {
                        var element = shape.Draw();
                        canvas.Children.Add(element);
                    }
                }

            }
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
            isPreview = false;

            if (selectedShape >= 0 || isBrushStroke)
            {
                if (selectedLayer >= 0)
                {
                    // Thêm đối tượng cuối cùng vào mảng quản lí
                    Point pos = e.GetPosition(canvas);
                    preview.HandleEnd(pos.X, pos.Y);
                    project.UserLayer[selectedLayer].UserShapes.Add(preview.Clone());
                    project.IsSaved = false;
                    Title = "Paint - " + project.GetName() + "*";
                }
                else
                {
                    // Thêm đối tượng cuối cùng vào mảng quản lí
                    Point pos = e.GetPosition(canvas);
                    preview.HandleEnd(pos.X, pos.Y);
                    allLayers.Insert(0, new layerView(project.addNewLayer(), true));
                    project.UserLayer[project.UserLayer.Count - 1 ].UserShapes.Add(preview.Clone());
                    project.IsSaved = false;
                    Title = "Paint - " + project.GetName() + "*";
                }

                // Sinh ra đối tượng mẫu kế
                preview = preview.NextShape();
                reDraw();
                Undo_Btn.IsEnabled = true;
                Redo_Btn.IsEnabled = false;
                list_project.Add(project.Clone());
                undo_project.Clear();
            }
           

        }

        private void ShapeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isBrushStroke = false;
            Brush_Stroke_Btn.IsChecked = false;
            isAddText = false;
            Add_Text_Btn.IsChecked = false;
            Edit_Text_Tab.Visibility = Visibility.Hidden;
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

        private void Open_Fill_ColorPicker_Click(object sender, RoutedEventArgs e)
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
                currentFillColor = picker.SelectedBrush;
                Color_Fill_Preview.Fill = currentFillColor;
                window.Close();
            };
            picker.Canceled += delegate { window.Close(); };
            window.Show(Open_Fill_ColorPicker, false);
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
            if(project.Address.Length > 0)
            {
                project.SaveToFile();
                Title = "Paint - " + project.GetName();
                return;
            }
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
                    allLayers.Clear();
                    for (int i = project.UserLayer.Count - 1; i>=0; i--)
                    {
                        var tempt = new layerView()
                        {
                            isVisible = project.UserLayer[i].isVisible,
                            name = project.UserLayer[i].name
                        };
                        allLayers.Add(tempt);

                    }
                    
                    reDraw();
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
            undo_project.Clear();

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

        private void copyLayer(Project prj1, Project prj2)
        {
            foreach (var layer in prj1.UserLayer)
            {
                prj2.UserLayer.Add(layer);
                int count = prj2.UserLayer.Count;
                for(int i = 0; i < count; i++)
                {
                    prj2.UserLayer[i].UserShapes = prj1.UserLayer[i].UserShapes;
                }    
            }
        }

        private void Undo_Btn_Click(object sender, RoutedEventArgs e)
        {
            int count = list_project.Count;
            if (count > 1 )
            {
                Redo_Btn.IsEnabled = true;
                undo_project.Add(list_project[count - 1].Clone());
                project = list_project[count - 2].Clone();

                allLayers.Clear();
                for (int i = project.UserLayer.Count - 1; i >= 0; i--)
                {
                    var tempt = new layerView()
                    {
                        isVisible = project.UserLayer[i].isVisible,
                        name = project.UserLayer[i].name
                    };
                    allLayers.Add(tempt);

                }
                reDraw();

                list_project.RemoveAt(count - 1);

                if (list_project.Count == 1)
                {
                    Undo_Btn.IsEnabled = false;
                }
            }

        }

        private void Redo_Btn_Click(object sender, RoutedEventArgs e)
        {
            int count = undo_project.Count;
            if (count > 0)
            {
                Undo_Btn.IsEnabled = true;
                list_project.Add(undo_project[count - 1].Clone());
                project = undo_project[count - 1].Clone();

                allLayers.Clear();
                for (int i = project.UserLayer.Count - 1; i >= 0; i--)
                {
                    var tempt = new layerView()
                    {
                        isVisible = project.UserLayer[i].isVisible,
                        name = project.UserLayer[i].name
                    };
                    allLayers.Add(tempt);

                }

                reDraw();

                undo_project.RemoveAt(count - 1);

                if (undo_project.Count == 0)
                {
                    Redo_Btn.IsEnabled = false;
                }
            }

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

        private void DragOverLayerList(object sender, DragEventArgs e)
        {

        }

        private void DropLayerList(object sender, DragEventArgs e)
        {

        }

        private void LayerList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("LAYER"))
            {
                layerView droppedData = e.Data.GetData("LAYER") as layerView;
                layerView target = ((ListViewItem)(sender)).DataContext as layerView;
                if (droppedData.Equals(target)) return;

                int removedIdx = allLayers.IndexOf(droppedData);
                int targetIdx = allLayers.IndexOf(target);

                int removedLayerIdx = allLayers.Count - 1 - (removedIdx);
                int targetLayerIdx = allLayers.Count - 1 - (targetIdx);

                if (removedIdx < 0 || targetIdx < 0 || removedIdx > allLayers.Count || targetIdx > allLayers.Count)
                {
                    return;
                }
                else if (removedIdx < targetIdx)
                {
                    var temp = project.UserLayer[removedLayerIdx];
                    project.UserLayer.RemoveAt(removedLayerIdx);
                    project.UserLayer.Insert(targetLayerIdx,temp);
                    allLayers.Insert(targetIdx + 1, droppedData);
                    allLayers.RemoveAt(removedIdx);

                    project.IsSaved = false;
                    list_project.Add(project.Clone());
                    undo_project.Clear();
                }
                else
                {
                    int remIdx = removedIdx + 1;
                    if (allLayers.Count + 1 > remIdx)
                    {
                        project.UserLayer.Insert(targetLayerIdx + 1, project.UserLayer[removedLayerIdx]);
                        project.UserLayer.RemoveAt(removedLayerIdx);

                        allLayers.Insert(targetIdx, droppedData);
                        allLayers.RemoveAt(remIdx);
                    }
                    project.IsSaved = false;
                    list_project.Add(project.Clone());
                    undo_project.Clear();
                }
            }
            reDraw();
        }

        private void LayerList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListViewItem)
            {
                ListViewItem draggedItem = sender as ListViewItem;
                DataObject data = new DataObject("LAYER", draggedItem.DataContext);
                DragDrop.DoDragDrop(draggedItem, data, DragDropEffects.Move);
                draggedItem.IsSelected = true;
            }
        }

        private void LayerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LayerList.SelectedIndex >= 0)
            {
                selectedLayer = allLayers.Count - 1 - LayerList.SelectedIndex;
            }
            else
            {
                selectedLayer = -1;
            }
            
        }

        private void AddNewLayer_Click(object sender, RoutedEventArgs e)
        {
            allLayers.Insert(0,new layerView(project.addNewLayer(), true));
            project.IsSaved = false;
            list_project.Add(project.Clone());
            undo_project.Clear();
        }

        private void Zoom_In_Btn_Click(object sender, RoutedEventArgs e)
        {
            if(st.ScaleX <= 5 && st.ScaleY <= 5)
            {
                st.ScaleX *= 1.25;
                st.ScaleY *= 1.25;
                Zoom_Slider.Value = st.ScaleX * 100;
                ZoomValue = Zoom_Slider.Value;
            }
        }

        private void Zoom_100_Btn_Click(object sender, RoutedEventArgs e)
        {
            st.ScaleX = 1;
            st.ScaleY = 1;
            Zoom_Slider.Value = st.ScaleX * 100;
            ZoomValue = Zoom_Slider.Value;
        }

        private void Zoom_Out_Btn_Click(object sender, RoutedEventArgs e)
        {
            if(st.ScaleX >= 0.25 && st.ScaleY >= 0.25)
            {
                st.ScaleX *= 0.8;
                st.ScaleY *= 0.8;
                Zoom_Slider.Value = st.ScaleX * 100;
                ZoomValue = Zoom_Slider.Value;
            }
        }

        private void Zoom_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ZoomValue = Zoom_Slider.Value;
            st.ScaleX = ZoomValue / 100;
            st.ScaleY = ZoomValue / 100;
        }

        private void CutContractKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (project.Address.Length > 0)
                {
                    project.SaveToFile();
                    Title = "Paint - " + project.GetName();
                    return;
                }
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
            else if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control)
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
            else if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
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
                            }
                        }
                        project.SaveToFile();
                    }
                    else if (msgResult == MessageBoxResult.Cancel)
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
                    if (temProject == null)
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

                        reDraw();
                    }
                }
            }
            else if (e.Key == Key.Tab)
            {
                canfocus = false;
            }
            else if (e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control)
            {
                int count = list_project.Count;
                if (count > 1)
                {
                    Redo_Btn.IsEnabled = true;
                    undo_project.Add(list_project[count - 1].Clone());
                    project = list_project[count - 2].Clone();

                    allLayers.Clear();
                    for (int i = project.UserLayer.Count - 1; i >= 0; i--)
                    {
                        var tempt = new layerView()
                        {
                            isVisible = project.UserLayer[i].isVisible,
                            name = project.UserLayer[i].name
                        };
                        allLayers.Add(tempt);

                    }
                    reDraw();

                    list_project.RemoveAt(count - 1);

                    if (list_project.Count == 1)
                    {
                        Undo_Btn.IsEnabled = false;
                    }
                }
            }
            else if (e.Key == Key.Y && Keyboard.Modifiers == ModifierKeys.Control)
            {
                int count = undo_project.Count;
                if (count > 0)
                {
                    Undo_Btn.IsEnabled = true;
                    list_project.Add(undo_project[count - 1].Clone());
                    project = undo_project[count - 1].Clone();

                    allLayers.Clear();
                    for (int i = project.UserLayer.Count - 1; i >= 0; i--)
                    {
                        var tempt = new layerView()
                        {
                            isVisible = project.UserLayer[i].isVisible,
                            name = project.UserLayer[i].name
                        };
                        allLayers.Add(tempt);

                    }

                    reDraw();

                    undo_project.RemoveAt(count - 1);

                    if (undo_project.Count == 0)
                    {
                        Redo_Btn.IsEnabled = false;
                    }
                }
            }

        }

        private void Open_Recent_File_Btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = HandyControl.Controls.MessageBox.Show(new MessageBoxInfo
            {
                Message = "Open recent file...",
                Caption = "Code open recent file here",
                Button = MessageBoxButton.OK,
                IconBrushKey = ResourceToken.SuccessBrush,
                IconKey = ResourceToken.SuccessGeometry,
                StyleKey = "MessageBoxCustom"
            });

        }


        private void DeleteLayer_Click(object sender, RoutedEventArgs e)
        {
            while (LayerList.SelectedItems.Count > 0)
            {

                project.UserLayer.RemoveAt(allLayers.Count - 1 - LayerList.SelectedIndex);
                allLayers.RemoveAt(LayerList.SelectedIndex);

                selectedLayer = -1;
                project.IsSaved = false;
                list_project.Add(project.Clone());
                undo_project.Clear();
                reDraw();

            }

           
        }

        private void isVisibleUncheck(object sender, RoutedEventArgs e)
        {
            CheckBox b = sender as CheckBox;
            layerView rule = b.CommandParameter as layerView;
            int index = allLayers.IndexOf(rule);

            project.UserLayer[allLayers.Count - 1 - index].isVisible = false;
            reDraw();
        }

        private void isVisibleCheck(object sender, RoutedEventArgs e)
        {
            CheckBox b = sender as CheckBox;
            layerView rule = b.CommandParameter as layerView;
            int index = allLayers.IndexOf(rule);

            project.UserLayer[allLayers.Count - 1 - index].isVisible = true;
            reDraw();
        }

        private void LayerList_PreviewMouseMove(object sender, MouseEventArgs e)
        {

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is ListViewItem)
                {
                    ListViewItem draggedItem = sender as ListViewItem;
                    DataObject data = new DataObject("LAYER", draggedItem.DataContext);
                    DragDrop.DoDragDrop(draggedItem, data, DragDropEffects.Move);
                    draggedItem.IsSelected = true;
                }
            }
        }

       
        private void Brush_Stroke_Btn_Click(object sender, RoutedEventArgs e)
        {
            ShapeList.SelectedIndex = -1;
            isBrushStroke = true;
            Brush_Stroke_Btn.IsChecked = true;
            isAddText = false;
            Add_Text_Btn.IsChecked = false;
            Edit_Text_Tab.Visibility = Visibility.Hidden;
            preview = new BrushStroke();
        }


        private void Canvas_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                string[] droppedFilePaths = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                foreach (var path in droppedFilePaths)
                {
                    var converter = new ImageSourceConverter();
                    ImageSource imageSource = (ImageSource)converter.ConvertFromString(path);
                    if (imageSource != null)
                    {
                        Image2D img = new Image2D();
                        Point s = e.GetPosition(canvas);
                        img.HandleStart(s.X, s.Y);
                        img.HandleEnd(s.X + imageSource.Width, s.Y + imageSource.Height);
                        img._source = imageSource;
                        canvas.Children.Add(img.Draw());

                        if (selectedLayer >= 0)
                        {
                            // Thêm đối tượng cuối cùng vào mảng quản lí
                            project.UserLayer[selectedLayer].UserShapes.Add(img.Clone());
                            project.IsSaved = false;
                            Title = "Paint - " + project.GetName() + "*";
                            list_project.Add(project.Clone());
                            undo_project.Clear();
                            Undo_Btn.IsEnabled = true;
                        }
                        else
                        {
                            // Thêm đối tượng cuối cùng vào mảng quản lí
                            allLayers.Insert(0, new layerView(project.addNewLayer(), true));
                            project.UserLayer[project.UserLayer.Count - 1].UserShapes.Add(img.Clone());
                            project.IsSaved = false;
                            Title = "Paint - " + project.GetName() + "*";
                            list_project.Add(project.Clone());
                            undo_project.Clear();
                            Undo_Btn.IsEnabled = true;

                        }

                    }
                }
                
                
            }
        }

        private void Add_Text_Btn_Click(object sender, RoutedEventArgs e)
        {
            ShapeList.SelectedIndex = -1;
            isBrushStroke = false;
            Brush_Stroke_Btn.IsChecked = false;
            isAddText = true;
            Add_Text_Btn.IsChecked = true;
            Edit_Text_Tab.Visibility = Visibility.Visible;
            Edit_Text_Tab.IsSelected = true;
        }

        private void Open_Foreground_Picker_Click(object sender, RoutedEventArgs e)
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
                currentTextForeground = picker.SelectedBrush;
                Foreground_Preview.Fill = currentTextForeground;
                window.Close();
            };
            picker.Canceled += delegate { window.Close(); };
            window.Show(Open_Foreground_Picker, false);
        }

        private void Open_Background_Picker_Click(object sender, RoutedEventArgs e)
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
                currentTextBackground = picker.SelectedBrush;
                Background_Preview.Fill = currentTextBackground;
                window.Close();
            };
            picker.Canceled += delegate { window.Close(); };
            window.Show(Open_Background_Picker, false);
        }

        private void Font_Combo_Box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentFontFamily = (FontFamily)Font_Combo_Box.SelectedItem;
        }

        private void Font_Size_Combo_Box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentFontSize = (int)Font_Size_Combo_Box.SelectedItem;
        }

        private void Bold_Btn_Click(object sender, RoutedEventArgs e)
        {
            isBold = !isBold;
        }

        private void Italic_Btn_Click(object sender, RoutedEventArgs e)
        {
            isItalic = !isItalic;
        }

        private void Underline_Btn_Click(object sender, RoutedEventArgs e)
        {
            isUnderline = !isUnderline;
            if (isUnderline)
            {
                Strikethrough_Btn.IsChecked = false;
                isStrike = false;
            }
        }

        private void canvas_LostFocus(object sender, RoutedEventArgs e)
        {
            canfocus = false;
        }

        private void RibbonWindow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            canfocus = false;
        }

        private void GroupLayer_Click(object sender, RoutedEventArgs e)
        {
            bool[] layerCheck = new bool[maxLayerAmount];
            int insertIndex = LayerList.SelectedIndex;
            int allLayer = allLayers.Count;
            int UserLayerCount = project.UserLayer.Count;
            List<Layer> tempLayer = new List<Layer>();

            while (LayerList.SelectedItems.Count > 0)
            {
                layerCheck[LayerList.SelectedIndex ] = true;
                LayerList.SelectedItems.RemoveAt(0);
                selectedLayer = -1;

            }

           

            for (int i = UserLayerCount - 1; i >= 0; i--)
            {
                if (layerCheck[i] == true)
                {
                    tempLayer.Add(project.UserLayer[UserLayerCount - 1 - i]);
                    allLayers.RemoveAt(i);
                }
            }

            for (int i = 0; i < UserLayerCount; i++)
            {
                if (layerCheck[i] == true)
                {
                    project.UserLayer.RemoveAt(UserLayerCount - 1 - i);
                    
                }
            }


         

            Layer groupLayer = LayerFactory.GroupLayer(tempLayer);
            layerView tempView = new layerView()
            {
                isVisible = groupLayer.isVisible,
                name = groupLayer.name

            };


            int insertTo = insertIndex;
            for (int i = 0; i < insertTo; i++)
            {
                if (layerCheck[i] == true)
                {
                    insertIndex--;

                }
            }
            allLayers.Insert(insertIndex, tempView);

            int insertLayerIndex = allLayers.Count - 1 - insertIndex;
            

            project.UserLayer.Insert(insertLayerIndex, groupLayer);

            project.IsSaved = false;
            list_project.Add(project.Clone());
            undo_project.Clear();


            reDraw();
        }

        private void UngroupLayer_Click(object sender, RoutedEventArgs e)
        {

            while (LayerList.SelectedItems.Count > 0)
            {
                int UserLayerIndex = allLayers.Count - 1 - LayerList.SelectedIndex;
                int AlllayerIndex = LayerList.SelectedIndex;
                List<Layer> ungroupLayer = LayerFactory.UngroupLayer(project.UserLayer[UserLayerIndex]);
                project.UserLayer.RemoveAt(UserLayerIndex);
                allLayers.RemoveAt(AlllayerIndex);
                UserLayerIndex--;
                AlllayerIndex--;

                if (UserLayerIndex < 0) { UserLayerIndex = 0; }
                if (AlllayerIndex < 0) { AlllayerIndex = 0; }

                foreach (var layer in ungroupLayer)
                {
                    

                    layerView tempView = new layerView()
                    {
                        isVisible = layer.isVisible,
                        name = layer.name
                    };

                    allLayers.Insert(AlllayerIndex, tempView);

                    project.UserLayer.Insert(allLayers.Count - 1 - AlllayerIndex, layer);
                    UserLayerIndex++;
                    AlllayerIndex++;
                }



                selectedLayer = -1;
                project.IsSaved = false;
                list_project.Add(project.Clone());
                undo_project.Clear();
                reDraw();

            }
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Strikethrough_Btn_Click(object sender, RoutedEventArgs e)
        {
            isStrike = !isStrike;
            if (isStrike)
            {
                Underline_Btn.IsChecked = false;
                isUnderline = false;
            }
        }
    }
}
