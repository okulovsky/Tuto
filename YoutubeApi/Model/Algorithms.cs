using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeApi.Model
{
    public class Algorithms
    {
        public static int Match(string s1, string s2)
        {
            var matrix=new int[s1.Length,s2.Length];
            var max = Math.Max(s1.Length, s2.Length);
            matrix[0, 0] = s1[0] == s2[0] ? 1 : 0;
            for (int i=1;i<=s1.Length+s2.Length;i++)
                for (int j = 0; j <= i; j++)
                {
                    var p1 = j;
                    var p2 = i - j;
                    if (p1 >= s1.Length || p2 >= s2.Length) continue;

                    var mid = int.MinValue;
                    var top = int.MinValue;
                    var left = int.MinValue;

                    if (p1 > 0)
                        top = matrix[p1 - 1, p2];
                    if (p2 > 0)
                        left = matrix[p1, p2 - 1];
                    if (p1 > 0 && p2 > 0)
                        mid = matrix[p1 - 1, p2 - 1] + (s1[p1] == s2[p2] ? 1 : 0);

                    matrix[p1, p2] = Math.Max(mid, Math.Max(top, left));
                }
            return matrix[s1.Length - 1, s2.Length - 1];
        }

        static double RelativeMatch(string s1, string s2)
        {
            return (2.0 * Match(s1,s2)) / (s1.Length + s2.Length);
        }
    }
}
