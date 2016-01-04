using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public class FileChunk
    {
        public bool StartsNewEpisode { get; set; }
        public int StartTime { get; set; }
        public int Length { get; set; }
        public Mode Mode { get; set; }  // using only Screen and Face
        // public int EndTime { get { return StartTime + Length; } }

        //public FileInfo SourceFilename { get; set; }  // model.Locations.FaceVideo or DesktopVideo
        public string AudioFilename { get { return String.Format("audio_{0}_{1}.avi", StartTime, Length); } }
        public string VideoFilename { get { return String.Format("video_{0}_{1}.avi", StartTime, Length); } }
        public string ChunkFilename { get { return String.Format("chunk_{0}_{1}.avi", StartTime, Length); } }
        
    }
}
