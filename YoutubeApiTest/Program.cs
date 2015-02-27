﻿/*
*/
using System.Linq;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Tuto.Publishing.Youtube;

namespace Google.Apis.YouTube.Samples
{
    /// <summary>
    /// YouTube Data API v3 sample: retrieve my uploads.
    /// Relies on the Google APIs Client Library for .NET, v1.7.0 or higher.
    /// See https://code.google.com/p/google-api-dotnet-client/wiki/GettingStarted
    /// </summary>
    internal class MyUploads
    {
        [STAThread]
        static void Main(string[] args)
        {
            var processor = new YoutubeApisProcessor();
            var directory = new DirectoryInfo(".");
            processor.Authorize(directory);
            var videos = processor.GetAllClips();
            foreach (var video in videos)
                Console.WriteLine("{0,-15}{1,10}", video.Id, video.Name);

            var v = videos.First();
			processor.UpdateVideoThumbnail(v, new FileInfo("aiml.png"));

			return;
			v.Name = "XXX";
            v.Description = "YYY";
            processor.UpdateVideo(v);

            var lists = processor.GetAllPlaylists();
            foreach (var list in lists)
            {
                Console.WriteLine("{0}", list.PlaylistTitle);
                processor.DeletePlaylist(list);
            }
            var newList = processor.CreatePlaylist("Test");
            processor.FillPlaylist(newList, videos);
        }

        private async Task Run()
        {
            UserCredential credential;
            
            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleSecrets.Data,
                new[] { YouTubeService.Scope.YoutubeReadonly },
                "user",
                CancellationToken.None,
                new FileDataStore(this.GetType().ToString())
            );
            

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.GetType().ToString()
            });

            var channelsListRequest = youtubeService.Channels.List("contentDetails");
            channelsListRequest.Mine = true;

            // Retrieve the contentDetails part of the channel resource for the authenticated user's channel.
            var channelsListResponse = await channelsListRequest.ExecuteAsync();

            foreach (var channel in channelsListResponse.Items)
            {
                // From the API response, extract the playlist ID that identifies the list
                // of videos uploaded to the authenticated user's channel.
                var uploadsListId = channel.ContentDetails.RelatedPlaylists.Uploads;

                Console.WriteLine("Videos in list {0}", uploadsListId);

                var nextPageToken = "";
                while (nextPageToken != null)
                {
                    var playlistItemsListRequest = youtubeService.PlaylistItems.List("snippet");
                    playlistItemsListRequest.PlaylistId = uploadsListId;
                    playlistItemsListRequest.MaxResults = 50;
                    playlistItemsListRequest.PageToken = nextPageToken;

                    // Retrieve the list of videos uploaded to the authenticated user's channel.
                    var playlistItemsListResponse = await playlistItemsListRequest.ExecuteAsync();

                    foreach (var playlistItem in playlistItemsListResponse.Items)
                    {
                        // Print information about each video.
                        Console.WriteLine("{0} ({1})", playlistItem.Snippet.Title, playlistItem.Snippet.ResourceId.VideoId);
                    }

                    nextPageToken = playlistItemsListResponse.NextPageToken;
                }
            }
        }
    }
}