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
using Path = System.IO.Path;
using Tuto.Navigator.ViewModels;


namespace Tuto.Navigator.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainNavigatorWindow : Window
    {
        public MainNavigatorWindow()
        {
            InitializeComponent();
        }

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			var MainModel = (MainModel)DataContext;
			if (MainModel == null) return;
			if (MainModel.EditorVisible)
			{
				e.Cancel = true;
				MainModel.CurrentVideo.Save();
				MainModel.CurrentVideo.WindowState.OnGetBack();
			}
		}


    }
}
