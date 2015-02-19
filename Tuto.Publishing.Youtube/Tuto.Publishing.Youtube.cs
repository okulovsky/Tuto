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

		static CourseTreeData Convert(GlobalData data)
		{
			var converted = new CourseTreeData();
			converted.Directory = data.GlobalDataFolder;
			converted.Videos = new List<VideoPublishSummary>();
			converted.Structure=new CourseStructure();
			foreach (var e in data.VideoData)
			{
				converted.Videos.Add(new VideoPublishSummary(e));
				if (e.TopicGuid != null)
					converted.Structure.VideoToTopicRelations.Add(new VideoToTopicRelation { VideoGuid = e.Guid, TopicGuid = e.TopicGuid, NumberInTopic = e.NumberInTopic });
			}
			converted.Structure.RootTopic = data.TopicsRoot;
			return converted;

		}

        [STAThread]
        public static void Main(string[] args)
        {
            var currentDirectoryName = EditorModelIO.SubstituteDebugDirectories(args[0]);
            currentDirectory = new DirectoryInfo(currentDirectoryName);

			var oldFormatFile = new FileInfo(Path.Combine(currentDirectory.FullName, "project.tuto"));
			var newFormatFile = new FileInfo(Path.Combine(currentDirectory.FullName, GlobalData.VideoListName));
			if (oldFormatFile.Exists && !newFormatFile.Exists)
			{
				var globalData = EditorModelIO.ReadGlobalData(currentDirectory);
				var structure = Convert(globalData);
				structure.Save();

			}


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
                (Application.MainWindow.DataContext as MainViewModel).Reload();
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
