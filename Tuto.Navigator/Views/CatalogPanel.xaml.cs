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

            Tree.AllowDrop = true;
            Tree.PreviewMouseLeftButtonDown += Tree_PreviewMouseLeftButtonDown;
            Unassigned.PreviewMouseLeftButtonDown += Unassigned_PreviewMouseLeftButtonDown;
            PreviewMouseLeftButtonUp += CmPreviewMouseLeftButtonUp;
            MouseMove += CmMouseMove;
            Tree.Drop += Tree_Drop;
            Tree.SelectedItemChanged += (s, a) =>
                {
                    if (DataContext != null)
                        ((PublishViewModel)DataContext).SelectedItem = Tree.SelectedValue as Wrap;

                };
        }


        Action makeDragDrop;

        void CmPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            makeDragDrop = null;
        }

        void CmMouseMove(object sender, MouseEventArgs e)
        {
            if (makeDragDrop != null)
            {
                makeDragDrop();
                makeDragDrop = null;
            }
        }

        const string format = "my format";

        void Tree_Drop(object sender, DragEventArgs e)
        {
            var placeRect = GetParentOfType<Rectangle>(e.GetPosition(this));
            if (placeRect == null) return;

            var placeArray  = Enum.GetValues(typeof(MoveDestination))
                .Cast<MoveDestination>()
                .Where(z=>z.ToString()==placeRect.Name)
                .ToArray();

            if (placeArray.Length==0) return;
            var place=placeArray[0];


            var dstNode = GetParentOfType<TreeViewItem>(e.GetPosition(this));
            if (dstNode == null) return;
            var dst = dstNode.DataContext;
            if (dst == null) return;
            if (!(dst is Wrap)) return;
            var destination = dst as Wrap;



            var context = DataContext as PublishViewModel;
            if (context == null) return;
            var what = (Wrap)e.Data.GetData(format);
            
            context.MoveToTree(what, destination, place);
        }



        T GetParentOfType<T>(Point locationRelativeToPanel)
            where T : class
        {
            var obj = VisualTreeHelper.HitTest(this, locationRelativeToPanel).VisualHit;
            while (true)
            {
                if (obj == null) return default(T);
                if (obj.GetType() == typeof(T)) return obj as T;
                obj = VisualTreeHelper.GetParent(obj);
            }
        }

        void DoDragAndDrop(FrameworkElement element)
        {
            if (element == null) return;
            if (element.DataContext == null) return;
            var data = new DataObject(format, element.DataContext);
            makeDragDrop = () => DragDrop.DoDragDrop(element, data, DragDropEffects.Move);
        }
     

        void Tree_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var testBox = GetParentOfType<TextBox>(e.GetPosition(this));
            if (testBox != null) return;
            var treeNode = GetParentOfType<TreeViewItem>(e.GetPosition(this));
            DoDragAndDrop(treeNode);
        }


        void Unassigned_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = GetParentOfType<ListBoxItem>(e.GetPosition(this));
            DoDragAndDrop(item);
        }
    }
}