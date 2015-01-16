using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;
using Tuto.Navigator;
using Tuto.Publishing.YoutubeData;

namespace Tuto.Publishing
{
    public partial class VideoWrap : VideoItem, ICommandBlocksHolder
    {
	
        public VideoWrap()
		{
			CommandBlocks = new List<ICommandBlockModel>();
		}

		public List<ICommandBlockModel> CommandBlocks
		{
			get;
			private set;
		}
	}
}
