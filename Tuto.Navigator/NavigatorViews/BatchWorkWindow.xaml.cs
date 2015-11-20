using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Tuto.BatchWorks;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Tuto.Navigator
{
    /// <summary>
    /// Interaction logic for BatchWorkWindow.xaml
    /// </summary>
    public partial class BatchWorkWindow : Window
    {
        public BatchWorkWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }

        public void AssignCancelOperation (Action<int> cancel)
        {
            CancelButton.Click += (s, a) =>
            {
                var index = Tasks.SelectedIndex;
                cancel(index);
            };
        }
    }
}
