using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using System.Reflection;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.Upload;
using System.IO;
using Tuto.Model;
using Google.Apis.Util.Store;

namespace Tuto.Publishing
{
    public class UploadVideo
    {

        private string clientSecretsPath { get; set; }
        private EditorModel model;
        private int episodeNumber;
        private EpisodInfo episode;
        private FileInfo pathToFile;
        

        public UploadVideo(EditorModel model, int number, FileInfo path)
        {
            this.model = model;
            this.episodeNumber = number;
            this.episode = model.Montage.Information.Episodes[number];
            this.pathToFile = path;
        }

        [STAThread]
        public void Proceed()
        {
            Console.WriteLine("YouTube Data API: Upload Video");
            try
            {
                new UploadVideo(model, episodeNumber, pathToFile).Run().Wait();
            }
            catch (AggregateException ex)
            {
                var msg = new List<string>();
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("Error: " + e.Message);
                    msg.Add(e.Message);
                }
                throw new ArgumentException(string.Join("\n", msg));
            }
        }

        private YouTubeService uploadService { get; set; }
        private async Task Run()
        {
            UserCredential uploadCredential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                uploadCredential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { YouTubeService.Scope.Youtube, YouTubeService.Scope.YoutubeUpload, YouTubeService.Scope.YoutubeReadonly },
                    "user",
                    CancellationToken.None
                    ,new FileDataStore("temp.json")
                );
            }

            uploadService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = uploadCredential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });


            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = episode.Name;
            //video.Snippet.Description = info.Guid.ToString(); //maybe description will be
            //video.Snippet.Tags = new string[] { "tag1", "tag2" }; //maybe tags will be
            video.Snippet.CategoryId = "22"; 
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "public";
            var filePath = pathToFile.FullName;
            if (episode.YoutubeId != null)
            {
                var request = uploadService.Videos.Delete(episode.YoutubeId);
                await request.ExecuteAsync();
            }

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = uploadService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;
                await videosInsertRequest.UploadAsync();
            }
        }

        public event EventHandler Uploaded;
        void videosInsertRequest_ResponseReceived(Video video)
        {
            Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
            episode.YoutubeId = video.Id;
            model.Save();
            if (Uploaded != null)
                Uploaded("", new EventArgs());
        }
    }
}
