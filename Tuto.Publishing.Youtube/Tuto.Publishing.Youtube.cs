using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tuto.Model;
using Tuto.Publishing.Youtube.Views;
using YoutubeApiTest;

namespace Tuto.Publishing.Youtube
{
    public static class TutoPublishingYoutubeProgram
    {
        static DirectoryInfo currentDirectory;
        static Application Application;

        [STAThread]
        public static void Main(string[] args)
        {
            var currentDirectoryName = EditorModelIO.SubstituteDebugDirectories(args[0]);
            currentDirectory = new DirectoryInfo(currentDirectoryName);
            Application = new System.Windows.Application();
            var model = new MainViewModel(currentDirectory, ()=>SourcesFactory());
            var window = new MainWindow();
            window.DataContext = model;
            Application.Run(window);
        }

        public static void RunCatalogWindow()
        {
            var globalData = EditorModelIO.ReadGlobalData(currentDirectory);
            var model = new Tuto.TreeEditor.PublishViewModel(globalData.TopicsRoot, globalData.VideoData);
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
                var globalData = EditorModelIO.ReadGlobalData(currentDirectory);
                globalData.TopicsRoot = data.Item1;
                globalData.VideoData = data.Item2;
                EditorModelIO.Save(globalData);
                (Application.MainWindow.DataContext as MainViewModel).Reload();
            }
            Application.MainWindow.Show();
        }

        static IEnumerable<IMaterialSource> SourcesFactory()
        {
            yield return new YoutubeSource();
           // yield return new LatexSource();
			yield return new ULearnSource();
        }

    }
}
