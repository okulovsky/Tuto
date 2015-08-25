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
        private EpisodeBindingInfo info { get; set; }

        public UploadVideo(EpisodeBindingInfo info)
        {
            this.info = info;
        }

        [STAThread]
        public void Proceed()
        {
            Console.WriteLine("YouTube Data API: Upload Video");
            Console.WriteLine("==============================");

            try
            {
                new UploadVideo(info).Run().Wait();
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

            Console.WriteLine("Press any key to continue...");
        }

        private async Task Run()
        {
            UserCredential uploadCredential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                uploadCredential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    // This OAuth 2.0 access scope allows an application to upload files to the
                    // authenticated user's YouTube channel, but doesn't allow other types of access. YouTubeService.Scope.YoutubeUpload, YouTubeService.Scope.YoutubeReadonly, 
                    new[] { YouTubeService.Scope.Youtube, YouTubeService.Scope.YoutubeUpload, YouTubeService.Scope.YoutubeReadonly },
                    "user",
                    CancellationToken.None
                    ,new FileDataStore("temp.json")
                );
            }

            var uploadService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = uploadCredential,
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });


            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = info.Title;
            video.Snippet.Description = info.Guid.ToString();
            video.Snippet.Tags = new string[] { "tag1", "tag2" };
            video.Snippet.CategoryId = "22"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "public"; // or "private" or "unlisted"
            var filePath = info.FullName; // Replace with path to actual movie file.

            var currentId = info.Model.Montage.Information.Episodes.Where(x => x.Guid == info.Guid).FirstOrDefault();
            if (currentId != null)
            {
                var request = uploadService.Videos.Delete(currentId.YoutubeId);
                await request.ExecuteAsync();
            }
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = uploadService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;
                await videosInsertRequest.UploadAsync();
            }
        }


        void videosInsertRequest_ResponseReceived(Video video)
        {
            Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
            var model = info.Model;
            foreach (var ep in model.Montage.Information.Episodes)
            {
                if (ep.Guid == info.Guid)
                {
                    ep.YoutubeId = video.Id;
                    model.Save();
                    return;
                }
            }
        }
    }
}
