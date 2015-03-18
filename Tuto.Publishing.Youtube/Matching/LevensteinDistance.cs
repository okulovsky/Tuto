using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing.Matching
{
    public static class LevensteinDistance
    {

		public static void Print(string s1, string s2, int[,] matrix)
		{
			for (int y = -1; y <= s2.Length; y++)
			{
				for (int x = -1; x <= s1.Length; x++)
				{
					if (x == -1 && y == -1) Console.Write("   ");
					else if (x == -1)
					{
						if (y == 0) Console.Write("   ");
						else Console.Write("{0,3}", s2[y - 1]);
					}
					else if (y == -1)
					{
						if (x == 0) Console.Write("   ");
						else Console.Write("{0,3}",s1[x-1]);
					}
					else Console.Write("{0,3}", matrix[x, y]);
				}
				Console.WriteLine();
			}
		}

        public static int[,] MatchNamesMatrix(string s1, string s2)
        {
            if (s1 == null || s2 == null) return null;
            var matrix = new int[s1.Length+1, s2.Length+1];
			for (int p1 = 0; p1 <= s1.Length; p1++)
				for (int p2 = 0; p2 <= s2.Length; p2++)
				{
					if (p1 == 0 && p2 == 0) continue;
					else if (p1 == 0)
						matrix[p1, p2] = matrix[p1, p2 - 1] + 1;
					else if (p2 == 0)
						matrix[p1, p2] = matrix[p1 - 1, p2] + 1;
					else if (s1[p1 - 1] == s2[p2 - 1])
						matrix[p1, p2] = matrix[p1 - 1, p2 - 1];
					else 
						matrix[p1, p2] = 1 + Math.Min(matrix[p1 - 1, p2], matrix[p1, p2 - 1]);
				}
			return matrix;
        }

		public static int MatchNames(string s1, string s2)
		{
			return MatchNamesMatrix(s1, s2)[s1.Length, s2.Length];
		}

		public static double RelativeDistance(string s1, string s2)
		{
			double matchResult =MatchNames(s1, s2); 
			return 1 - matchResult / (s1.Length + s2.Length);
		}

    }
}
