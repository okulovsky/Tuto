using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeApi.ViewModel
{
    public class Wrap
    {
        public Wrap Parent { get; set; }
        public List<Wrap> Children { get; private set; }
        public Wrap() { Children = new List<Wrap>(); }
    }
}
