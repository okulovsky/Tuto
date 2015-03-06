using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto
{
    public class SrtMaker
    {
        public static string MsInSrtFormat(int ms)
        {
            var result=string.Format("{0:hh}:{0:mm}:{0:ss},{0:fff}", new TimeSpan(0, 0, 0, 0, ms));
            return result;
        }

        public static int FindFinalLocation(int startTime, IEnumerable<StreamChunk> chunks)
        {
            var start = startTime;
            var rest = startTime;
            foreach (var c in chunks)
            {
                if (rest < c.Length) break;
                if (c.IsNotActive)
                    start -= c.Length;
                rest -= c.Length;
            }
            return start;
        }

        public static List<int> CumulativeEpisodesLengthes(EditorModel model)
        {
            var list = new List<int>();
            list.Add(0);
            list.Add(0);
            foreach (var e in model.Montage.Chunks)
            {
                if (e.StartsNewEpisode) list.Add(list[list.Count-1]);
                if (e.IsActive) list[list.Count - 1] += e.Length;
            }
            return list;
        }

        public static IEnumerable<SubtitleFix> ShiftFixes(EditorModel model)
        {
            foreach (var e in model.Montage.SubtitleFixes.OrderBy(z => z.StartTime))
            {
                yield return new SubtitleFix
                {
                    StartTime=FindFinalLocation(e.StartTime, model.Montage.Chunks),
                    Length=e.Length,
                    Text=e.Text
                };
            }
        }

        public static IEnumerable<string> CreateSrtFile(EditorModel model)
        {
            var episodesLength=CumulativeEpisodesLengthes(model);
            var subtitles = ShiftFixes(model);
            for (int i=0;i<episodesLength.Count-1;i++)
            {
                var builder = new StringBuilder();
                var number = 1;
                foreach(var fix in subtitles.Where(z=>z.StartTime>=episodesLength[i] && z.StartTime<=episodesLength[i+1]))
                    builder.AppendFormat("{0}\r\n{1} --> {2}\r\n{3}\r\n\r\n",
                            number++,
                    MsInSrtFormat(fix.StartTime-episodesLength[i]),
                    MsInSrtFormat(fix.StartTime-episodesLength[i]+fix.Length),
                    fix.Text);
                yield return builder.ToString();
            }
        }

        public static void WriteSrtFiles(EditorModel model)
        {
            int episode = 0;
            foreach(var str in CreateSrtFile(model))
            {
                File.WriteAllText(
                    model.Locations.GetSrtFile(episode).FullName,
                    str);
                episode++;
            }
        }
    }
}
