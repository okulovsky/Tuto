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

namespace Tuto.Init
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IVideothequeLoadingUI
	{
		public MainWindow()
		{
			InitializeComponent();
			OK.Click += OK_Click;
			Cancel.Click += Cancel_Click;
			DataContextChanged += MainWindow_DataContextChanged;
		}

        VideothequeRequestViewModel context { get { return ((VideothequeRequestViewModel)DataContext); } }

		void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			RequestPanel.Visibility = DataContext == null ? Visibility.Collapsed : Visibility.Visible;
		}

		void Cancel_Click(object sender, RoutedEventArgs e)
		{
            context.Cancelled = true;
			handle.Set();
		
		}

		void OK_Click(object sender, RoutedEventArgs e)
		{
            if (!context.OkIsEnabled) throw new Exception();
            context.Cancelled = false;
			handle.Set();
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
            var viewModel = new VideothequeRequestViewModel(prompt, items);

			handle = new AutoResetEvent(false);
			Dispatcher.BeginInvoke(new Action(() => DataContext = viewModel));
			handle.WaitOne();

			Dispatcher.BeginInvoke(new Action(() => DataContext = null));
            if (viewModel.Cancelled) return null;
            return viewModel.Items.First(z => z.Selected).Item;	
		}



		public void ExitSuccessfully()
		{
			Dispatcher.BeginInvoke(new Action(Close));
		}
	}
}
