using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;
using Tuto;
using Tuto.Navigator;
using Tuto.Publishing.YoutubeData;

namespace Tuto.Publishing
{

    public class LectureWrap : LectureItem, ICommandBlocksHolder
    {


		public List<ICommandBlockModel> CommandBlocks
		{
			get;
			private set;
		}

		public LectureWrap()
		{
			CommandBlocks = new List<ICommandBlockModel>();
		}
	}
}
