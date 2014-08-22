using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Tuto.Publishing.Youtube.Views
{
    /// <summary>
    /// Interaction logic for PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : Window
    {
        public PasswordWindow()
        {
            InitializeComponent();
            ok.Click += (s, a) => { Close(); };
        }
        public string Password { get { return password.Password; } }

        public static string GetPassword()
        {
            var wnd = new PasswordWindow();
            wnd.ShowDialog();
            return wnd.Password;
        }
    }
}
