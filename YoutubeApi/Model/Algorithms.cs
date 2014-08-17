using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;
using YoutubeApi.ViewModel;

namespace YoutubeApi.Model
{
    class Lists
    {
        public readonly List<FinishedVideo> Finished;
        public readonly List<PublishedVideo> Published;
        public readonly List<ClipData> Clips;
        public readonly List<VideoWrap> Result;
        public Lists(List<FinishedVideo> _finished, List<PublishedVideo> _published, List<ClipData> _clips)
        {
            Finished = _finished.ToList();
            Published = _published.ToList();
            Clips = _clips.ToList();
            Result=new List<VideoWrap>();
        }
        public void Account(VideoWrap wrap)
        {
            Result=new List<VideoWrap>();
            Finished.Remove(wrap.Finished);
            Published.Remove(wrap.Published);
            Clips.Remove(wrap.ClipData);
        }
    }


    public class Algorithms
    {
        public static int MatchNames(string s1, string s2)
        {
            var matrix = new int[s1.Length, s2.Length];
            var max = Math.Max(s1.Length, s2.Length);
            matrix[0, 0] = s1[0] == s2[0] ? 1 : 0;
            for (int i = 1; i <= s1.Length + s2.Length; i++)
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
                    if (p1 > 0 && p2 > 0)
                        mid = matrix[p1 - 1, p2 - 1] + (s1[p1] == s2[p2] ? 1 : 0);

                    matrix[p1, p2] = Math.Max(mid, Math.Max(top, left));
                }
            return matrix[s1.Length - 1, s2.Length - 1];
        }

        public static double RelativeMatchNames(string s1, string s2)
        {
            return (2.0 * MatchNames(s1, s2)) / (s1.Length + s2.Length);
        }
        
        #region Making match between videos
        const double MatchLowerLimit = 0.5;

        VideoWrap FindMatchThroughPub(Lists lists, FinishedVideo fin)
        {
            var pub = lists.Published.FirstOrDefault(z => z.Guid == fin.Guid);
            if (pub != null)
            {
                var clip = lists.Clips.FirstOrDefault(z => z.Id == pub.ClipId);
                return new VideoWrap(
                        fin,
                        pub,
                        clip,
                        clip == null ? Status.DeletedFromYoutube : Status.MatchedOld
                    );
            }
            return null;
        }

        VideoWrap FindNewMatch(Lists lists, FinishedVideo fin)
        {
            var bestMatch = lists.Clips
                        .Select(z => Tuple.Create(z, RelativeMatchNames(fin.Name, z.Name)))
                        .OrderByDescending(z => z.Item2)
                        .FirstOrDefault();
            //the match is being installed right now
            if (bestMatch != null && bestMatch.Item2 > MatchLowerLimit)
            {
                var pub1 = new PublishedVideo();
                pub1.Guid = fin.Guid;
                pub1.ClipId = bestMatch.Item1.Id;
                return new VideoWrap(fin, pub1, bestMatch.Item1, Status.MatchedNew);
            }
            return null;
        }

        public List<VideoWrap> MatchVideos(List<FinishedVideo> _finished, List<PublishedVideo> _published, List<ClipData> _clips)
        {
            var list = new Lists(_finished, _published, _clips);

            while (list.Finished.Count != 0)
            {
                var fin = list.Finished[0];
                var match = FindMatchThroughPub(list, fin);
                if (match == null) match = FindNewMatch(list, fin);
                if (match == null) match = new VideoWrap(fin, null, null, Status.NotFoundAtYoutube);
                list.Account(match);
            }

            while (list.Clips.Count != 0)
            {
                var clip=list.Clips[0];
                var pub = list.Published.FirstOrDefault(z => z.ClipId == clip.Id);
                list.Account(new VideoWrap(
                    null, 
                    pub, 
                    clip, 
                    pub==null?Status.NotExpectedAtYoutube:Status.DeletedFromTuto));
            }

            while (list.Published.Count != 0)
            {
                list.Account(new VideoWrap(null, list.Published[0], null, Status.DeletedFromBoth));
            }

            return list.Result;
        }
        #endregion

    }
}
 