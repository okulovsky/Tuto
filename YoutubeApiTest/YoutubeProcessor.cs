using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Tuto.Publishing;
using Tuto.Publishing.YoutubeData;

namespace YoutubeApiTest
{
    public static class AwaitableExtensions
    {
        public static T RunSync<T>(this Task<T> task)
        {
            task.Wait();
            return task.Result;
        }
    }

    class YoutubeApisProcessor : IYoutubeProcessor
    {
        YouTubeService service;
        UserCredential credential;
        const string CredentialsForlderName = "YoutubeApiCredentials";
        const string ProgramName = "Tuto Editor";

        public List<Tuto.Publishing.YoutubeClip> GetAllClips()
        {
            var videos = new List<YoutubeClip>();
            var channelsListRequest = service.Channels.List("contentDetails");
            channelsListRequest.Mine = true;

            // Retrieve the contentDetails part of the channel resource for the authenticated user's channel.
            var channelsListResponse = channelsListRequest.Execute();

            foreach (var channel in channelsListResponse.Items)
            {
                // From the API response, extract the playlist ID that identifies the list
                // of videos uploaded to the authenticated user's channel.
                var uploadsListId = channel.ContentDetails.RelatedPlaylists.Uploads;

                var nextPageToken = "";
                while (nextPageToken != null)
                {
                    var playlistItemsListRequest = service.PlaylistItems.List("snippet");
                    playlistItemsListRequest.PlaylistId = uploadsListId;
                    playlistItemsListRequest.MaxResults = 50;
                    playlistItemsListRequest.PageToken = nextPageToken;

                    // Retrieve the list of videos uploaded to the authenticated user's channel.
                    var playlistItemsListResponse = playlistItemsListRequest.Execute();

                    foreach (var playlistItem in playlistItemsListResponse.Items)
                    {
                        var snippet = playlistItem.Snippet;
                        videos.Add(new YoutubeClip { Id = snippet.ResourceId.VideoId, Name = snippet.Title, Description = snippet.Description });
                    }

                    nextPageToken = playlistItemsListResponse.NextPageToken;
                }
            }
            return videos;
        }
      

        public void UpdateVideo(Tuto.Publishing.YoutubeClip clip, YoutubeVideoUpdateInfo updateInfo)
        {
            
        }


        public void Authorize(DirectoryInfo directory)
        {
            var credentialsLocation = Path.Combine(directory.FullName, CredentialsForlderName);

            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleSecrets.Data,
                new[] { YouTubeService.Scope.YoutubeReadonly },
                "user",
                CancellationToken.None,
                new FileDataStore(credentialsLocation)).RunSync();

            service = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.GetType().ToString()
            });


        }
    }
}
