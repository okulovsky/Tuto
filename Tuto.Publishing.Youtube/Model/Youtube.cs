using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.YouTube;

namespace Tuto.Publishing.Youtube.Model
{
    public class YoutubeProcessor
    {
        public static string Password;
        public static string Username;
        public static string DeveloperKey;
        public static string ChannelId;
        public static string ApplicationName = "Tuto Publisher";
        public List<ClipData> Load()
        {
            var list=new List<ClipData>();
            var settings = new YouTubeRequestSettings(ApplicationName, DeveloperKey, Username, Password);
            var request = new YouTubeRequest(settings);
            var videos = request.GetVideoFeed(ChannelId);
            foreach (var v in videos.Entries)
                list.Add(new ClipData
                {
                    Id = v.Id,
                    Name = v.Title
                });
            return list;
        }

        public void UpdateClipData(ClipData clipData)
        {

        }

    }
}
