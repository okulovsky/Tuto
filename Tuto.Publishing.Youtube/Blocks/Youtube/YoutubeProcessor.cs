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

namespace Tuto.Publishing.Youtube
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
		const string GuidMarker = "GUID: ";

        public List<Tuto.Publishing.YoutubeClip> GetAllClips()
        {
            var videos = new List<YoutubeClip>();
            var channelsListRequest = service.Channels.List("contentDetails");
            channelsListRequest.Mine = true;

            // Retrieve the contentDetails part of the channel resource for the authenticated user's channel.
            var channelsListResponse = channelsListRequest.Execute();

			var ids = new List<string>();

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
						ids.Add(snippet.ResourceId.VideoId);
                        //videos.Add(new YoutubeClip { Id = snippet.ResourceId.VideoId, Name = snippet.Title, Description = snippet.Description });
                    }

                    nextPageToken = playlistItemsListResponse.NextPageToken;
                }
            }

			var result = new List<YoutubeClip>();
				

			int takeCount = 50;
			for (int skip = 0; skip < ids.Count; skip += takeCount)
			{
				var listRq = service.Videos.List("snippet");
				var idsList = ids.Skip(skip).Take(takeCount).Aggregate((a, b) => a + "," + b);
				listRq.Id = idsList;
				var allVideos = listRq.Execute().Items;

				foreach (var e in allVideos)
				{
					var clip = new YoutubeClip();
					clip.Id = e.Id;
					clip.Name = e.Snippet.Title;
					clip.Description = e.Snippet.Description;

					string guidMark = null;
					if (e.Snippet.Tags != null) e.Snippet.Tags.Where(z => z.StartsWith(GuidMarker)).FirstOrDefault();
					if (guidMark != null)
					{
						Guid guid;
						guidMark = guidMark.Substring(GuidMarker.Length);
						if (Guid.TryParse(guidMark, out guid))
							clip.StoredGuid = guid;
					}

					result.Add(clip);

				}
			}

            return result;
        }


        public void UpdateVideo(YoutubeClip clip)
        {
            var listRq = service.Videos.List("snippet");
            listRq.Id = clip.Id;
            var video = listRq.Execute().Items[0];
            video.Snippet.Title = clip.Name;
            video.Snippet.Description = clip.Description;
			if (clip.StoredGuid.HasValue)
				video.Snippet.Tags = new List<string> { GuidMarker+clip.StoredGuid.Value.ToString() };
			else
				video.Snippet.Tags = null;
			service.Videos.Update(video, "snippet").Execute();
        }

		public void UpdateVideoThumbnail(YoutubeClip clip, FileInfo image)
		{
			// Create an API request that specifies that the mediaContent
			// object is the thumbnail of the specified video.
			using (var stream = File.Open(image.FullName, FileMode.Open))
			{
				var thumbnailSet = service.Thumbnails.Set(clip.Id, stream, "image/png");
				thumbnailSet.Upload();
			}
		}

        public YoutubePlaylist CreatePlaylist(string name)
        {
            var list = new Playlist();
            list.Snippet=new PlaylistSnippet();
            list.Snippet.Title = name;
            list.Status = new PlaylistStatus();
            list.Status.PrivacyStatus = "public";
            list=service.Playlists.Insert(list,"snippet,status").Execute();
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
