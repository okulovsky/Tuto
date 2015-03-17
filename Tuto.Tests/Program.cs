using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Tests
{
	class Program
	{
		public static void Main()
		{
			var s1 = "xyazubxcz";
			var s2 = "fafgbfdch";
			var matrix = Tuto.Publishing.Matching.LevensteinDistance.MatchNamesMatrix(s1, s2);
			Tuto.Publishing.Matching.LevensteinDistance.Print(s1, s2, matrix);
		}
	}
}
