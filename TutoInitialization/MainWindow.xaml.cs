using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tuto.Model;
using Tuto.Model;

namespace Tuto.NewMainWindow
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IVideothequeLoadingUI
	{
		public MainWindow()
		{
			InitializeComponent();
			Options.SelectionChanged += Options_SelectionChanged;
			OK.Click += OK_Click;
			Cancel.Click += Cancel_Click;
			DataContextChanged += MainWindow_DataContextChanged;
		}

		void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			RequestPanel.Visibility = DataContext == null ? Visibility.Collapsed : Visibility.Visible;
		}

		void Cancel_Click(object sender, RoutedEventArgs e)
		{
			((VideothequeRequestViewModel)DataContext).SelectedItem = null;
			handle.Set();
		
		}

		void OK_Click(object sender, RoutedEventArgs e)
		{
			handle.Set();
		}

		void Options_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			((VideothequeRequestViewModel)DataContext).SelectedItem = (VideothequeLoadingRequestItem)Options.SelectedItem;
		}

		public void StartPOSTWork(string name)
		{
			Dispatcher.BeginInvoke(
				new Action(() => this.Report.Text += name + "...")
				);
		}

		public void CompletePOSTWork(bool result)
		{
			Dispatcher.BeginInvoke(
				new Action(() => this.Report.Text += (result?"OK":"FAILED") + "\r\n")
				);
		}


		AutoResetEvent handle;
		public VideothequeLoadingRequestItem Request(string prompt, VideothequeLoadingRequestItem[] items)
		{
			var viewModel = new VideothequeRequestViewModel
			{
				Prompt=prompt,
				Items=items
			};
			handle = new AutoResetEvent(false);
			Dispatcher.BeginInvoke(new Action(() => DataContext = viewModel));
			handle.WaitOne();

			return viewModel.SelectedItem;
		}



		public void ExitSuccessfully()
		{
			Dispatcher.BeginInvoke(new Action(Close));
		}
	}
}
