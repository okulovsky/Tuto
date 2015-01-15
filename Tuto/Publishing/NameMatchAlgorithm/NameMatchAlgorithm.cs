using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
    public static class NameMatchAlgorithm
    {
        public static int MatchNames(string s1, string s2)
        {
            if (s1 == null || s2 == null) return 0;
            var matrix = new int[s1.Length, s2.Length];
            var max = Math.Max(s1.Length, s2.Length);

            for (int i = 0; i <= s1.Length + s2.Length; i++)
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

                    mid = (s1[p1] == s2[p2] ? 1 : 0);
                    if (p1 > 0 && p2 > 0)
                        mid += matrix[p1 - 1, p2 - 1];

                    var mmm = Math.Max(mid, Math.Max(top, left));
                    matrix[p1, p2] = mmm;
                }
            return matrix[s1.Length - 1, s2.Length - 1];
        }

        public static double RelativeMatchNames(string s1, string s2)
        {
            if (s1 == null || s2 == null) return 0;
            var match = MatchNames(s1, s2);
            return (2.0 * match) / (s1.Length + s2.Length);
        }

        public static TData FindBest<TData>(string etalon, IEnumerable<TData> data, Func<TData, string> selector, double threashold=0.2)
        {
            return data
                .Select(z => new { Data = z, Caption = selector(z) })
                .Select(z => new { Data = z.Data, Metric = RelativeMatchNames(etalon, z.Caption) })
				.Where(z=>z.Metric>threashold)
                .OrderByDescending(z => z.Metric)
                .Select(z=>z.Data)
                .FirstOrDefault();

        }
    }
}
