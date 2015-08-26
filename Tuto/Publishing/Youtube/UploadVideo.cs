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
            this.info.Model = EditorModelIO.Load(info.Model.VideoFolder.FullName);
            this.info.EpisodeInfo = GetNewEpisodeInfo();

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
                    // This OAuth 2.0 access scope allows an application to upload files to the
                    // authenticated user's YouTube channel, but doesn't allow other types of access. YouTubeService.Scope.YoutubeUpload, YouTubeService.Scope.YoutubeReadonly, 
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
            video.Snippet.Title = info.EpisodeInfo.Name;
            video.Snippet.Description = info.EpisodeInfo.Guid.ToString();
            video.Snippet.Tags = new string[] { "tag1", "tag2" };
            video.Snippet.CategoryId = "22"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "public"; // or "private" or "unlisted"
            var filePath = info.FullName;


            try
            {
                if (info.EpisodeInfo.YoutubeId != null)
                {
                    var request = uploadService.Videos.Delete(info.EpisodeInfo.YoutubeId);
                    await request.ExecuteAsync();

                    if (info.EpisodeInfo.ItemPlaylistId != null)
                    {
                        var playlistItemsListRequest = uploadService.PlaylistItems.Delete(info.EpisodeInfo.ItemPlaylistId);
                        var playlistItemsListResponse = await playlistItemsListRequest.ExecuteAsync();
                        info.EpisodeInfo.ItemPlaylistId = null;
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


        void videosInsertRequest_ResponseReceived(Video video)
        {
            Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
            info.EpisodeInfo.YoutubeId = video.Id;
            AddToPlaylist(video.Id);
        }

        private EpisodInfo GetNewEpisodeInfo()
        {
            foreach (var ep in this.info.Model.Montage.Information.Episodes)
            {
                if (ep.Guid == info.EpisodeInfo.Guid)
                {
                    return ep;
                }
            }
            return null;
        }

        private async void AddToPlaylist(string id)
        {
            //move to playlist
            if (info.EpisodeInfo.PlaylistId != null)
            {
                var newPlaylistItem = new PlaylistItem();
                newPlaylistItem.Snippet = new PlaylistItemSnippet();
                newPlaylistItem.Snippet.PlaylistId = info.EpisodeInfo.PlaylistId;
                newPlaylistItem.Snippet.ResourceId = new ResourceId();
                newPlaylistItem.Snippet.ResourceId.Kind = "youtube#video";
                newPlaylistItem.Snippet.ResourceId.VideoId = id;
                newPlaylistItem = await uploadService.PlaylistItems.Insert(newPlaylistItem, "snippet").ExecuteAsync();
                info.EpisodeInfo.ItemPlaylistId = newPlaylistItem.Id;
                info.Model.Save();
            }
        }
    }
}
