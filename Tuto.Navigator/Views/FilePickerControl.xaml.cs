using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;

namespace Tuto.Navigator
{
    /// <summary>
    /// Логика взаимодействия для FilePickerControl.xaml
    /// </summary>
    public partial class FilePickerControl : UserControl
    {
        public FilePickerControl()
        {
            InitializeComponent();
            OpenCommand = new RelayCommand(Open);
        }

        public void Open()
        {
            var dialog = new OpenFileDialog
            {
                Filter = FileFilter,
                FilterIndex = 0,
            };
            var result = dialog.ShowDialog();
            if (!(result.HasValue && result.Value))
                return;
            FilePath = dialog.FileName;
        }

        public RelayCommand OpenCommand { get; private set; }

        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(FilePickerControl), new PropertyMetadata(""));

      

        public string FileFilter
        {
            get { return (string)GetValue(FileFilterProperty); }
            set { SetValue(FileFilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileFilterProperty =
            DependencyProperty.Register("FileFilter", typeof(string), typeof(FilePickerControl), new PropertyMetadata("All files|*.*"));

        
    }
}
