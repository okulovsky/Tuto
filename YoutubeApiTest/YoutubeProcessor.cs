using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
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

    public class YoutubeApisProcessor : IYoutubeProcessor
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


        public void UpdateVideo(YoutubeClip clip)
        {
            var listRq = service.Videos.List("snippet");
            listRq.Id = clip.Id;
            var video = listRq.Execute().Items[0];
            video.Snippet.Title = clip.Name;
            video.Snippet.Description = clip.Description;
            service.Videos.Update(video, "snippet").Execute();
        }

        public YoutubePlaylist CreatePlaylist(string name)
        {
            var list = new Playlist();
            list.Snippet=new PlaylistSnippet();
            list.Snippet.Title = name;
            list=service.Playlists.Insert(list,"snippet").Execute();
            var playlist = new YoutubePlaylist();
            playlist.PlaylistId = list.Id;
            playlist.PlaylistTitle = list.Snippet.Title;
            return playlist;
        }

        public List<YoutubePlaylist> GetAllPlaylists()
        {
            var listRequest = service.Playlists.List("snippet");
            listRequest.Mine=true;
            var lists = listRequest.Execute();
            return lists.Items.Select(z => new YoutubePlaylist { PlaylistId = z.Id, PlaylistTitle = z.Snippet.Title }).ToList();
        }

        public void DeletePlaylist(YoutubePlaylist playlist)
        {
            service.Playlists.Delete(playlist.PlaylistId).Execute();
        }

        public void FillPlaylist(YoutubePlaylist list, IEnumerable<YoutubeClip> clips)
        {
            var itemsRq = service.PlaylistItems.List("snippet");
            itemsRq.PlaylistId = list.PlaylistId;
            var items = itemsRq.Execute().Items;
            foreach (var e in items)
                service.PlaylistItems.Delete(e.Id);

            foreach (var e in clips)
            {
                var item = new PlaylistItem
                {
                    Snippet = new PlaylistItemSnippet
                    {
                        PlaylistId = list.PlaylistId,
                        ResourceId = new ResourceId
                        {
                            Kind = "youtube#video",
                            VideoId = e.Id
                        }
                    }
                };
                service.PlaylistItems.Insert(item,"snippet").Execute();
            }
        }

        public void Authorize(DirectoryInfo directory)
        {
            var credentialsLocation = Path.Combine(directory.FullName, CredentialsForlderName);

            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleSecrets.Data,
                new[] { YouTubeService.Scope.Youtube },
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
