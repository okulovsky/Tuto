using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace YoutubeApi.Model
{
    class PublishedVideo
    {
        public FinishedVideo Video { get; private set; }
        public string ClipId { get; set; }
    }
}
