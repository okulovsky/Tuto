using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tuto.Model;
using Tuto.Publishing.Youtube.Views;

namespace Tuto.Publishing.Youtube
{
    public static class TutoPublishingYoutubeProgram
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var directory = EditorModelIO.SubstituteDebugDirectories(args[0]);
            var passwordFile = new FileInfo(Path.Combine(directory, "password"));
            string password="";
            if (passwordFile.Exists)
                password = File.ReadAllText(passwordFile.FullName);
            else
                password = PasswordWindow.GetPassword();
            
            
            var folder=new DirectoryInfo(directory);
            var model = new MainViewModel();
            model.GlobalData=EditorModelIO.ReadGlobalData(folder);
            model.YoutubeSettings = HeadedJsonFormat.Read<YoutubeSettings>(folder);
            model.YoutubeProcessor = new YoutubeProcessor(model.YoutubeSettings, password);
            var treeRoot = ItemTreeBuilder.Build<FolderWrap, LectureWrap, VideoWrap>(model.GlobalData);
            model.Root = new[] { treeRoot };
            //var clips = new List<ClipData>();


            //try
            //{
            //    clips = youtubeProcessor.LoadVideos();
            //}
            //catch
            //{
            //    MessageBox.Show("Loading video from Youtube failed.");
            //}
            
            //var match = Algorithms.MatchVideos(globalData.VideoData, youtubeData.Videos, clips);
            //var root = Algorithms.CreateTree(globalData.TopicsRoot, match, globalData.TopicLevels);

            //var playlists = youtubeProcessor.FindPlaylists();
            //Algorithms.AddPlaylists(root, youtubeData.Topics, playlists);

            //var model = new MainViewModel(folder, globalData, youtubeData.Settings, root, youtubeProcessor, match);
            //var window = new MainWindow();
            //window.DataContext = model;
            //new Application().Run(window);
        }
    }
}
