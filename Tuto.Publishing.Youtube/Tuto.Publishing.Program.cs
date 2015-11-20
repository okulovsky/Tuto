using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tuto.Model;
using Tuto.Publishing.Youtube.Views;
using Tuto.Publishing.Youtube;

namespace Tuto.Publishing
{
    public static class TutoPublishingYoutubeProgram
    {
        static DirectoryInfo currentDirectory;
        static Application Application;

        [STAThread]
        public static void Main(string[] args)
        {
			if (args.Length==0)
			{
				MessageBox.Show("This program should be called with an argument: the working directory or the arbitrary file in this directory");
				return;
			}

			var currentDirectory = new DirectoryInfo(args[0]);
            Application = new System.Windows.Application();
            var model = new MainViewModel(currentDirectory, ()=>SourcesFactory());
            var window = new MainWindow();
            window.DataContext = model;
            Application.Run(window);
        }

        public static void RunCatalogWindow()
        {
			var globalData = CourseTreeData.Load(currentDirectory);
			var model = new Tuto.TreeEditor.PublishViewModel(globalData);
            var wnd = new Tuto.TreeEditor.PublishPanel();
            wnd.DataContext = model;
            wnd.Closed += wnd_Closed;
            Application.MainWindow.Hide();
            wnd.Show();
        }

        static void wnd_Closed(object sender, EventArgs e)
        {
            var catalog = (sender as Window).DataContext as Tuto.TreeEditor.PublishViewModel;
            var response = MessageBox.Show("Save changes?", "Tuto.Publishing", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (response != MessageBoxResult.No)
            {
                var data = catalog.Commit();
				HeadedJsonFormat.Write(currentDirectory, data);               
                (Application.MainWindow.DataContext as MainViewModel).ReloadOld();
            }
            Application.MainWindow.Show();
        }

        static IEnumerable<IMaterialSource> SourcesFactory()
        {
            yield return new YoutubeSource();
            yield return new LatexSource();
			yield return new ULearnSource();
        }

    }
}
