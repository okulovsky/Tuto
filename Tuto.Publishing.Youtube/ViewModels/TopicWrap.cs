using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Publishing.Youtube
{
    public class TopicWrap : Wrap
    {
        public Topic Topic { get; set; }
        public TopicLevel CorrespondedLevel { get; set; }
        public override int Digits
        {
            get
            {
                return CorrespondedLevel.Digits;
            }
        }

    }
}
