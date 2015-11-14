using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    public static partial class EditorModelIO
    {
        static Videotheque UpdateGlobalV0(FileInfo file, string text)
        {
            var globalData = HeadedJsonFormat.ReadWithoutHeader<Videotheque>(text);
            globalData.TopicsRoot = new Topic();
            globalData.TopicLevels = new List<TopicLevel>();
            globalData.VideoData = new List<FinishedVideo>();
            return globalData;
        }
    }
}