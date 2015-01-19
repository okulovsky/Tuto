using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;
using Tuto;
using Tuto;
using Tuto.Publishing.YoutubeData;

namespace Tuto.Publishing
{

    public class LectureWrap : LectureItem, ICommandBlocksHolder, IExpandingDataHolder
    {

        public ExpandingData ExpandingData
        {
            get { return this.GetExpandingData(); }
        }

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
