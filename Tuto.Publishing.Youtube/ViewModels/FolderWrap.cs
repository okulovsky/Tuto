using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Publishing
{

    public class FolderWrap : FolderItem, IExpandingDataHolder
    {
        public ExpandingData ExpandingData
        {
            get { return this.GetExpandingData(); }
        }
    }
}
