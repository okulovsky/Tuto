using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    public static class BorderListExtensions
    {
        public static int FindBorder(this List<Border> list, int ms)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i].IsInside(ms))
                    return i;
            return -1;
        }
    }
}
