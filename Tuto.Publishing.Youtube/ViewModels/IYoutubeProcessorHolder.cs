using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Publishing.YoutubeData;

namespace Tuto.Publishing
{
    interface IYoutubeProcessorHolder : IItem
    {
        IYoutubeProcessor Processor { get; set; }
    }
}
