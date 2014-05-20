using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public class EpisodInfoV4
    {
        public string Name { get; set; }
        public Guid LectureId { get; set; }
        public Guid AuthorId { get; set; }
        public TimeSpan Duration { get; set; }
        public int NumberInTopic { get; set; }
    }
}
