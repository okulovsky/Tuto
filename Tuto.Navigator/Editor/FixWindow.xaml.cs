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

namespace Editor.Windows
{
    /// <summary>
    /// Interaction logic for FixWindow.xaml
    /// </summary>
    public partial class FixWindow : Window
    {
        public FixWindow()
        {
            InitializeComponent();
            Ok.Click += (s, a) => { DialogResult=true; Close(); };
            PreviewKeyDown += (s, a) =>
                {
                    if (a.Key == Key.Enter) { DialogResult = true; }
                    else if (a.Key == Key.Escape) { DialogResult = false; }
                };
            Text.Focus();
        }

        public static string EnterText(string text)
        {
            var wnd = new FixWindow();
            wnd.Text.Text = text;
            wnd.ShowDialog();
            if (wnd.DialogResult.HasValue && wnd.DialogResult.Value) return wnd.Text.Text;
            return text;

        }
    }
}
