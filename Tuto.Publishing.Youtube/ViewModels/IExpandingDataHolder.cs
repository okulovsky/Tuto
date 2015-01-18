using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
    interface IExpandingDataHolder : IItem
    {
        ExpandingData ExpandingData { get; }
    }

    static class ExpandingDataHolderExtensions
    {
        public static ExpandingData GetExpandingData(this IExpandingDataHolder holder)
        {
            var data = holder.Get<ExpandingData>();
            if (data == null)
            {
                data = new ExpandingData();
                holder.Store(data);
            }
            return data;
        }
    }
}
