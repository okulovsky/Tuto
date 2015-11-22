using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tuto.Model;
using Tuto.Model;

namespace Tuto.Init
{
	/// <summary>
	/// Interaction logic for RequestItemControl.xaml
	/// </summary>
	public partial class RequestItemControl : System.Windows.Controls.UserControl
	{
		public RequestItemControl()
		{
			InitializeComponent();
			DataContextChanged += RequestItemControl_DataContextChanged;
			BrowseButton.Click += BrowseButton_Click;
		}

		void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			var context = (VideothequeItemViewModel)DataContext;
			if (context == null) throw new Exception();
			if (context.Item.Type == VideothequeLoadingRequestItemType.NoFile) throw new Exception();

			string fname = null;

			if (context.Item.Type==  VideothequeLoadingRequestItemType.OpenFile)
			{
				var wnd = new System.Windows.Forms.OpenFileDialog();
				var result = wnd.ShowDialog();
				if (result == DialogResult.OK) 
				{
					fname = wnd.FileName;

				}
			}
			if (context.Item.Type == VideothequeLoadingRequestItemType.SaveFile)
			{
				var wnd = new System.Windows.Forms.SaveFileDialog();
				var result = wnd.ShowDialog();
				if (result == DialogResult.OK)
				{
					fname = wnd.FileName;
				}
			}
			if (context.Item.Type == VideothequeLoadingRequestItemType.Directory)
			{
				var wnd = new FolderBrowserDialog();
				 var result = wnd.ShowDialog();
				 if (result == DialogResult.OK)
				 {
					 fname = wnd.SelectedPath;
				 }
			}
	
			if (fname!=null)
			{
				SuggestedPath.Text = fname;
				context.Item.SuggestedPath = fname;
				context.Selected = true;
			}

		}

		void RequestItemControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var context = (VideothequeItemViewModel)DataContext;
			if (context.Item.Type == VideothequeLoadingRequestItemType.NoFile)
				this.BrowsePanel.Visibility = System.Windows.Visibility.Collapsed;
		}



		
	}
}
