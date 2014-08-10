using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.GData.Client;
using Google.YouTube;

namespace YoutubeApi
{
    class Program
    {

        static void Main(string[] args)
        {
            var key = File.ReadAllText("DeveloperKey.txt");
            YouTubeRequestSettings settings = new YouTubeRequestSettings("Tuto Editor", key);
            YouTubeRequest request = new YouTubeRequest(settings);
            var videos = request.GetVideoFeed("UCDpdqJALXjOkCmCD4Il7U7A");
            foreach (var v in videos.Entries) Console.WriteLine(v);

        }
    }
}
