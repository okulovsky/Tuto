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
            Application.Run(CreateMainWindow());
        }

        public static void RunCatalogWindow()
        {
            var globalData = EditorModelIO.ReadGlobalData(currentDirectory);
            var model = new Tuto.TreeEditor.PublishViewModel(globalData.TopicsRoot, globalData.VideoData);
            var wnd = new Tuto.TreeEditor.PublishPanel();
            wnd.DataContext = model;
            wnd.Closed += wnd_Closed;
            Application.MainWindow.Close();
            Application.MainWindow = wnd;
            wnd.Show();
        }

        static void wnd_Closed(object sender, EventArgs e)
        {
            var oldWindow = Application.MainWindow;
            Application.MainWindow = CreateMainWindow();
            Application.MainWindow.Show();
            oldWindow.Close();
        }


        public static MainWindow CreateMainWindow()
        {
            var globalData = EditorModelIO.ReadGlobalData(currentDirectory);
            var root = ItemTreeBuilder.Build<FolderWrap, LectureWrap, VideoWrap>(globalData);


            var settings = HeadedJsonFormat.Read<PublishingSettings>(currentDirectory);
            settings.Location = currentDirectory;
            if (settings == null) settings = new PublishingSettings();
			var sources = new List<IMaterialSource>();
			sources.Add(new YoutubeSource());
            sources.Add(new LatexSource());
			foreach (var s in sources)
				s.Initialize(settings);
            var model = new MainViewModel(settings, root, sources);

        
            var window = new MainWindow();
            window.DataContext = model;
            return window;
        }
    }
}
