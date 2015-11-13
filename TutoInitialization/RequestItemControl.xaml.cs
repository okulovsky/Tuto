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
			var context = (VideothequeLoadingRequestItem)DataContext;
			if (context == null) throw new Exception();
			if (context.Type == VideothequeLoadingRequestItemType.NoFile) throw new Exception();
			if (context.Type==  VideothequeLoadingRequestItemType.OpenFile)
			{
				var wnd = new System.Windows.Forms.OpenFileDialog();
				var result = wnd.ShowDialog();
				if (result == DialogResult.OK)
				{
					context.SuggestedPath = wnd.FileName;
				
				}
			}
			if (context.Type == VideothequeLoadingRequestItemType.SaveFile)
			{
				var wnd = new System.Windows.Forms.SaveFileDialog();
				var result = wnd.ShowDialog();
				if (result == DialogResult.OK)
				{
					context.SuggestedPath = wnd.FileName;
				}
			}
			if (context.Type == VideothequeLoadingRequestItemType.Directory)
			{
				var wnd = new FolderBrowserDialog();
				 var result = wnd.ShowDialog();
				 if (result == DialogResult.OK)
				 {
					 context.SuggestedPath = wnd.SelectedPath;
				 }
			}
			SuggestedPath.Text = context.SuggestedPath;

		}

		void RequestItemControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var context = (VideothequeLoadingRequestItem)DataContext;
			if (context.Type == VideothequeLoadingRequestItemType.NoFile)
				this.BrowsePanel.Visibility = System.Windows.Visibility.Collapsed;
		}



		
	}
}
