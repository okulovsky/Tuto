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
using Tuto.Model;

namespace Tuto.Navigator
{

    /// <summary>
    /// Interaction logic for RubrikatorControl.xaml
    /// </summary>
    public partial class PublishPanel : UserControl
    {
        public PublishPanel()
        {
            InitializeComponent();
            //Tree.SelectedItemChanged += (s, a) => { if (DataContext != null) ((PublishViewModel)DataContext).SelectedItem = (Topic)a.NewValue; };
            //DataContextChanged += (s1, a1) =>
            //{
            //    ((TopicsViewModel)DataContext).Updated += (s, a) => { Tree.Items.Refresh(); };
            //};

        }
    }
}