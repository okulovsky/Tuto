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
		static Videotheque videotheque;
		static PublishingModel publishingModel;

        [STAThread]
        public static void Main(string[] args)
        {
			if (args.Length==0)
			{
				MessageBox.Show("This program should be called with an argument: the path to videotheque file");
				return;
			}

            videotheque = Videotheque.Load(args[0], null, true);
            publishingModel = videotheque.PublishingModels.First();
			
            Application = new System.Windows.Application();
            var viewModel = new MainViewModel(videotheque,publishingModel,()=>SourcesFactory());
            var window = new MainWindow();
            window.DataContext = viewModel;
            Application.Run(window);
        }

        public static void RunCatalogWindow()
        {
			var videos = publishingModel.NonDistributedVideos.Concat(publishingModel.Videos).ToList();
			var globalData = new CourseTreeData { Structure = publishingModel.CourseStructure, Videos = videos };
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
				publishingModel.CourseStructure = data;
				publishingModel.Save();
				videotheque.UpdateNonDistributedVideos();
		        (Application.MainWindow.DataContext as MainViewModel).Reload();
            }
            Application.MainWindow.Show();
        }

        static IEnumerable<IMaterialSource> SourcesFactory()
        {
            yield return new YoutubeSource();
    		yield return new ULearnSource();
        }

    }
}
