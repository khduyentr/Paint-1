﻿using HandyControl.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Paint
{
    class SaveAsBPM: ISaveStrategy
    {
        public string Extension { get => "BPM"; }

        public void Execute(SaveFileDialog saveFileDialog, Project project, Canvas canvas, MainWindow window)
        {
            saveFileDialog.FileName = project.GetName();
            saveFileDialog.DefaultExt = ".bmp";
            saveFileDialog.Filter = "BMP files(*.bmp)|*.bmp";

            if (saveFileDialog.ShowDialog() == true)
            {
                Helper.SaveCanvasToFile((int)window.Canvas_Border.ActualWidth, (int)window.Canvas_Border.ActualHeight, canvas, saveFileDialog.FileName);
                HandyControl.Controls.MessageBox.Show(new MessageBoxInfo
                {
                    Message = "Export as " + Extension + " file successfully!",
                    Caption = "Paint",
                    Button = MessageBoxButton.OK,
                    IconBrushKey = ResourceToken.SuccessBrush,
                    IconKey = ResourceToken.SuccessGeometry,
                    StyleKey = "MessageBoxCustom"
                });
            }
        }
    }
}
