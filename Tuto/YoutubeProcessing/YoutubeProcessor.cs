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

    public class YoutubeApisProcessor
    {
        public static YoutubeApisProcessor Current { get; private set; }

        public static void Initialize(DirectoryInfo directory)
        {
            var processor = new YoutubeApisProcessor();
            var credentialsLocation = Path.Combine(directory.FullName, CredentialsForlderName);
            var clientId = "329852726670-mlvs6ephqo2vngr04t9t6q1d33dbi1g0.apps.googleusercontent.com";
            var clientSecret = "TVl9yVgWmsH5bfaB1jymooFV";

            processor.credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets {  ClientId=clientId, ClientSecret=clientSecret },
                new[] { YouTubeService.Scope.Youtube },
                "user",
                CancellationToken.None,
                new FileDataStore(credentialsLocation)).RunSync();

            processor.service = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = processor.credential,
                ApplicationName = "Tuto Editor"
            });
            Current = processor;
        }

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
			try
			{
				service.Playlists.Delete(playlist.PlaylistId).Execute();
			}
			catch { }
		}

        public void FillPlaylist(YoutubePlaylist list, IEnumerable<YoutubeClip> clips)
        {
            var itemsRq = service.PlaylistItems.List("snippet");
            itemsRq.PlaylistId = list.PlaylistId;
            var items = itemsRq.Execute().Items;
            foreach (var e in items)
                service.PlaylistItems.Delete(e.Id).Execute();

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

		public void DeleteVideo(string id)
		{
            try
            {
                service.Videos.Delete(id).ExecuteAsync().RunSync(); //если видео нет, то упадет, надо проверить существование
            }
            catch { }
		}

        private long fileSize;
        private Action<int> percentageUpdate;

        public string UploadVideo(FileInfo path, string name, Guid guid, Action<int> percentageUpdate)
		{
			var video = new Video();
			video.Snippet = new VideoSnippet();
			video.Snippet.Title = name;
			//video.Snippet.Description = info.Guid.ToString(); //maybe description will be
			//video.Snippet.Tags = new string[] { "tag1", "tag2" }; //maybe tags will be
			video.Snippet.CategoryId = "22";
			video.Snippet.Description = YoutubeClip.GuidMarker(guid);
			video.Status = new VideoStatus();
			video.Status.PrivacyStatus = "public";
			var filePath = path.FullName;

            fileSize = path.Length; //нужно для расчета прогресса
            this.percentageUpdate = percentageUpdate; //"обновлятель" свойства

			string result = null;
            
            using (var fileStream = new FileStream(filePath, FileMode.Open))
			{
				var videosInsertRequest = service.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ProgressChanged += OnProgressChanged;
				videosInsertRequest.ResponseReceived += v => { result = v.Id; };
                videosInsertRequest.UploadAsync().RunSync();
               
			}
			return result;
		}


        private void OnProgressChanged(Google.Apis.Upload.IUploadProgress obj)
        {
            if (percentageUpdate != null)
            {
                var percentage = (int)((double)obj.BytesSent / fileSize * 100);
                percentageUpdate(percentage);
            }
        }
    }
}
