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

namespace Tuto.Publishing.Youtube.Views
{
    /// <summary>
    /// Interaction logic for Tree.xaml
    /// </summary>
    public partial class Tree : UserControl
    {
        public Tree()
        {
            InitializeComponent();
        }

        public object SelectedItem
        {
            get { return InnerTree.SelectedItem; }
        }

        public event RoutedPropertyChangedEventHandler<object> SelectedItemChanged
        {
            add { InnerTree.SelectedItemChanged += value; }
            remove { InnerTree.SelectedItemChanged -= value; }
        }

    }
}
