using HandyControl.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace Paint
{
    public interface ISaveStrategy
    {
        public string Extension { get;}
        public void Execute(SaveFileDialog diaglog, Project project, Canvas canvas, MainWindow windows);
    }
}
