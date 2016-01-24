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

namespace Tuto.Navigator.Editor
{
    /// <summary>
    /// Interaction logic for EditorPanel.xaml
    /// </summary>
    public partial class EditorPanel : UserControl
    {
        EditorController controller;
        EditorModel model;
        public EditorPanel()
        {
            InitializeComponent();
            DataContextChanged += EditorPanel_DataContextChanged;
            backButton.Click += backButton_Click;
        }

        void backButton_Click(object sender, RoutedEventArgs e)
        {
            if (model != null && model.WindowState.GetBack != null)
                model.WindowState.GetBack();
        }

        void EditorPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (controller != null) controller.Dispose();
            model = DataContext as EditorModel;
            if (DataContext!=null)
            {
                controller = new EditorController(this.player, model);
         
            }
        }

    }
}
