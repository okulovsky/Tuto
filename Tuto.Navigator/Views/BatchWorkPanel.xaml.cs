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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tuto.Navigator.ViewModels;

namespace Tuto.Navigator.Views
{
    /// <summary>
    /// Interaction logic for BatchWorkPanel.xaml
    /// </summary>
    public partial class BatchWorkPanel : UserControl
    {
        public BatchWorkPanel()
        {
            InitializeComponent();
        }

        //costyl because TreeView doesn't have SelectedItem, so casual binding doesn't work
        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ((BatchWorkQueueViewModel)DataContext).SelectedWork = (BatchWorks.BatchWork)e.NewValue;
        }
    }
}
