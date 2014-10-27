using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Publishing.Youtube
{


    public static class IEnumerableStringExtensions
    {
        public static string JoinStrings(this IEnumerable<string> en, Func<string, string, string> op)
        {
            string result = "";
            foreach (var e in en)
            {
                if (result == "") result += e;
                else result = op(result, e);
            }
            return result;

        }
    }


    public class Algorithms
    {
        public static int MatchNames(string s1, string s2)
        {
            if (s1 == null || s2 == null) return 0;
            var matrix = new int[s1.Length, s2.Length];
            var max = Math.Max(s1.Length, s2.Length);

            for (int i = 0; i <= s1.Length + s2.Length; i++)
                for (int j = 0; j <= i; j++)
                {
                    var p1 = j;
                    var p2 = i - j;
                    if (p1 >= s1.Length || p2 >= s2.Length) continue;

                    var mid = int.MinValue;
                    var top = int.MinValue;
                    var left = int.MinValue;

                    if (p1 > 0)
                        top = matrix[p1 - 1, p2];
                    if (p2 > 0)
                        left = matrix[p1, p2 - 1];

                    mid=(s1[p1] == s2[p2] ? 1 : 0);
                    if (p1 > 0 && p2 > 0)
                        mid +=matrix[p1 - 1, p2 - 1];

                    var mmm=Math.Max(mid, Math.Max(top, left));
                    matrix[p1, p2] = mmm;
                }
            return matrix[s1.Length - 1, s2.Length - 1];
        }

        public static double RelativeMatchNames(string s1, string s2)
        {
            if (s1 == null || s2 == null) return 0; 
            var match = MatchNames(s1, s2);
            return (2.0 * match) / (s1.Length + s2.Length);
        }
        
        #region Making match between videos
     

        public static List<VideoWrap> MatchVideos(List<FinishedVideo> _finished, List<PublishedVideo> _published, List<ClipData> _clips)
        {
            var join = new Join3<FinishedVideo, PublishedVideo, ClipData, VideoWrap>();
            join.Inner = _finished.OrderBy(z=>z.Name).ToList();
            join.Middle = _published.ToList();
            join.Outer = _clips.OrderBy(z=>z.Name).ToList();
            join.InnerComparator = (a, b) => a.Guid == b.Guid;
            join.OuterComparator = (a, b) => a.Id == b.ClipId;
            join.CreateLink = (a, b) => new PublishedVideo { ClipId = b.Id, Guid = a.Guid };
            join.CreateResult = (a, b, c, d) => new VideoWrap(a, b, c, d);
            join.GetMatch = (a, b) => RelativeMatchNames(a.Name, b.Name);
            return join.Run();
        }
        #endregion

        #region Work with topics

        static TopicWrap Create(Topic topic, List<VideoWrap> videos, int level, List<TopicLevel> levels)
        {
            var result = new TopicWrap();
            result.Topic = topic;
            int number = 0;
            foreach (var e in videos
                .Where(z => z.Finished != null && z.Finished.TopicGuid == topic.Guid)
                .OrderBy(z => z.Finished.NumberInTopic))
            {
                e.NumberInTopic = number++;
                e.Parent = result;
                result.Children.Add(e);
            }



            if (level >= 0 && level < levels.Count)
                result.CorrespondedLevel = levels[level];
            else
                result.CorrespondedLevel = new TopicLevel { Caption = "", Digits = 2 };

            number = 0;
            foreach (var e in topic.Items)
            {
                var t = Create(e, videos, level + 1, levels);
                t.NumberInTopic = number++;
                t.Parent = result;
                result.Children.Add(t);
            }
            return result;
        }

        public static void AddPlaylists(TopicWrap root, List<PublishedTopic> published, List<PlaylistData> playlists)
        {
            root.PlaylistRequired = root.Children.OfType<VideoWrap>().Any();
            var pub = published.Where(z => z.TopicGuid == root.Topic.Guid).FirstOrDefault();
            if (pub != null)
            {
                root.Published = pub;
                root.Playlist = playlists.Where(z => z.Id == pub.PlaylistId).FirstOrDefault();
            }
            foreach (var e in root.Children.OfType<TopicWrap>())
                AddPlaylists(e, published, playlists);
        }

        public static TopicWrap CreateTree(Topic root, List<VideoWrap> videos, List<TopicLevel> levels)
        {
            var result=Create(root, videos, -1, levels);
            result.Root = true;
            return result;
        }

        #endregion

        #region Updating VideoWrap

        public static string GetAbbreviation(GlobalData globalData, VideoWrap wrap)
        {
            var numvers = wrap.PathFromRoot
                    .Where(z => !z.Root)
                    .Select(z => z.FormattedNumberInTopic)
                    .ToArray();

            return
                globalData.CourseAbbreviation
                + "-"
                + numvers.JoinStrings((a, b) => a + "-" + b)
                + " "
                + wrap.Finished.Name;
        }

        public static string GetDescription(GlobalData globalData, VideoWrap wrap)
        {
            var topics= wrap.TopicsFromRoot.Select(z =>
                    z.CorrespondedLevel.Caption
                    + " "
                    + (z.NumberInTopic + 1)
                    + "."
                    + z.Topic.Caption).ToArray();

            var str = "" + globalData.DescriptionPS;
            if (!string.IsNullOrEmpty(str)) str += "\r\n";
            return str
                + topics.JoinStrings((a, b) => a + "\r\n" + b);
        }


        #endregion

    }
}
 