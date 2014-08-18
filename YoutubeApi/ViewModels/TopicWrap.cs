using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace YoutubeApi.ViewModel
{
    class TopicWrap : Wrap
    {
        public Topic Topic { get; set; }
        public TopicLevel CorrespondedLevel { get; set; }
    }
}
