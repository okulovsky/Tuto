using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.GData.YouTube;
using Google.YouTube;
using Tuto.Model;

namespace Tuto.Publishing
{
    public class YoutubeProcessor
    {
        YoutubeSettings data;
        string password;
        const string ApplicationName = "Tuto Publisher";

        public YoutubeProcessor(YoutubeSettings settings, string password)
        {
            this.data = settings;
            this.password = password;
        }

        #region Videos

        private YouTubeRequest GetRequest()
        {

            var settings = new YouTubeRequestSettings(ApplicationName, data.DeveloperKey, data.Username, password);
            var request = new YouTubeRequest(settings);
            return request;
        }

        public List<YoutubeClip> LoadVideos()
        {
            var list = new List<YoutubeClip>();

            var request = GetRequest();


            int maxResults=50;
            int clashCounter=0;

            for (int startIndex = 0; ; startIndex +=maxResults/5)
            {
                var url = string.Format("http://gdata.youtube.com/feeds/api/users/{0}/uploads?sort=da&max-results={1}",
                data.ChannelUserId,
                maxResults);
                if (startIndex != 0)
                    url += "&start-index=" + startIndex;


                var videos = request.Get<Video>(new Uri(url));
                var hasEntries = false;

                foreach (var v in videos.Entries)
                {
                    hasEntries = true;
                    if (!list.Any(z => z.Id == v.VideoId))
                        list.Add(new YoutubeClip
                        {
                            Id = v.VideoId,
                            Name = v.Title
                        });
                    else clashCounter++;
                }

                if (!hasEntries) break;
            }

            return list;
        }

        private Video GetVideo(string id, YouTubeRequest request)
        {
            var url = string.Format("http://gdata.youtube.com/feeds/api/users/{0}/uploads/{1}",
                data.ChannelUserId,
                id);

            var video = request.Retrieve<Video>(new Uri(url));
            return video;
        }

        public void UpdateClipData(GlobalData global, YoutubeClip clipData)
        {
            var request = GetRequest();

            var video = GetVideo(clipData.Id, request);
            video.Description = clipData.Description;
            video.Title = clipData.Name;
            video.Keywords = global.Keywords;
            request.Update(video);
        }





        #endregion

        #region Playlists

        public List<YoutubePlaylist> FindPlaylists()
        {
            var request = GetRequest();
            var entries = request.GetPlaylistsFeed(data.ChannelUserId);
            var list = new List<YoutubePlaylist>();
            foreach (var e in entries.Entries)
                list.Add(new YoutubePlaylist { PlaylistId = e.Id, PlaylistTitle = e.Title });
            return list;
        }

        public YoutubePlaylist CreatePlaylist(LectureWrap wrap)
        {
            var request = GetRequest();
            var playlist = new Playlist();
            playlist.Title = playlist.Summary = wrap.Topic.Caption;
            var url = string.Format("http://gdata.youtube.com/feeds/api/users/{0}/playlists", data.ChannelUserId);
            playlist = request.Insert(new Uri(url), playlist);
            return new YoutubePlaylist { PlaylistId = playlist.Id, PlaylistTitle=playlist.Title };
        }

        public void UpdatePlaylist(LectureWrap wrap)
        {
            var request = GetRequest();
            var playlists = request.GetPlaylistsFeed(data.ChannelUserId);
            var playlist = playlists.Entries.Where(z => z.Id == wrap.YoutubePlaylist.PlaylistId).FirstOrDefault();
            if (playlist == null) return;

            playlist.Summary = playlist.Title = wrap.Topic.Caption;
            request.Update(playlist);

            var entry = request.GetPlaylist(playlist);
            foreach (var e in entry.Entries)
                request.Delete(e);
            foreach (var e in wrap.Children
                .OfType<VideoWrap>()
                .Where(z => z.YoutubeClip != null)
                .OrderBy(z => z.NumberInTopic))
            {
                var id = GetVideo(e.YoutubeClip.Id, request).Id;
                var pm = new PlayListMember { Id = id };
                request.AddToPlaylist(playlist, pm);
            }
        }

        #endregion
    }
}
