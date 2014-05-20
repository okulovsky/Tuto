using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public static class BorderListExtensions
    {
        public static int FindBorder(this List<BorderV4> list, int ms)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i].IsInside(ms))
                    return i;
            return -1;
        }
    }
}
