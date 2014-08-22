using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.GData.YouTube;
using Google.YouTube;
using Tuto.Model;

namespace Tuto.Publishing.Youtube
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

        public List<ClipData> Load()
        {
            var list=new List<ClipData>();
            var settings = new YouTubeRequestSettings(ApplicationName, data.DeveloperKey, data.Username, password);
            var request = new YouTubeRequest(settings);


            int maxResults=50;
            int clashCounter=0;

            for (int startIndex = 0; ; startIndex += maxResults)
            {
                var url = string.Format("http://gdata.youtube.com/feeds/api/users/{0}/uploads?max-results={1}",
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
                        list.Add(new ClipData
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

        public void UpdateClipData(GlobalData global, ClipData clipData)
        {
            var settings = new YouTubeRequestSettings(ApplicationName, data.DeveloperKey, data.Username, password);
            var request = new YouTubeRequest(settings);

            var url = string.Format("http://gdata.youtube.com/feeds/api/users/{0}/uploads/{1}",
                data.ChannelUserId,
                clipData.Id);

            var video = request.Retrieve<Video>(new Uri(url));
            video.Description = clipData.Description;
            video.Title = clipData.Name;
            video.Keywords = global.Keywords;
            request.Update(video);
        }

    }
}
