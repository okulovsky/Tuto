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
        private FinishedVideo info { get; set; }

        public UploadVideo(FinishedVideo info)
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
            video.Snippet.Title = info.Name;
            video.Snippet.Description = info.Guid.ToString(); //maybe description will be
            video.Snippet.Tags = new string[] { "tag1", "tag2" }; //maybe tags will be
            video.Snippet.CategoryId = "22"; 
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "public";
            var patched = info.PatchedName;
            var filePath = info.FullName;
            if (File.Exists(patched))
                filePath = patched;
            try
            {
                if (info.YoutubeId != null)
                {
                    var request = uploadService.Videos.Delete(info.YoutubeId);
                    await request.ExecuteAsync();

                    if (info.ItemPlaylistId != null)
                    {
                        var playlistItemsListRequest = uploadService.PlaylistItems.Delete(info.ItemPlaylistId);
                        var playlistItemsListResponse = await playlistItemsListRequest.ExecuteAsync();
                        info.ItemPlaylistId = null;
                    }
                }
            }
            catch (Exception e) 
            { 
                var test = e; 
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
            info.YoutubeId = video.Id;
            AddToPlaylist(video.Id);
            if (Uploaded != null)
                Uploaded("", new EventArgs());
        }

        private async void AddToPlaylist(string id)
        {
            //move to playlist
            if (info.PlaylistId != null)
            {
                
                var newPlaylistItem = new PlaylistItem();
                newPlaylistItem.Snippet = new PlaylistItemSnippet();
                newPlaylistItem.Snippet.PlaylistId = info.PlaylistId;
                newPlaylistItem.Snippet.ResourceId = new ResourceId();
                if (info.PlaylistPosition != null) //just added
                    newPlaylistItem.Snippet.Position = int.Parse(info.PlaylistPosition); //because inserting after
                newPlaylistItem.Kind = "youtube#playlistItem";
                newPlaylistItem.Snippet.ResourceId.Kind = "youtube#video";
                newPlaylistItem.Snippet.ResourceId.VideoId = id;
                newPlaylistItem = await uploadService.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync();

                //got null, so reload to get position
                var a = uploadService.PlaylistItems.List("snippet");
                a.Id = newPlaylistItem.Id;
                var ans = await a.ExecuteAsync();
                

                info.ItemPlaylistId = newPlaylistItem.Id;
                info.PlaylistPosition = ans.Items[0].Snippet.Position.ToString();
            }
        }
    }
}
