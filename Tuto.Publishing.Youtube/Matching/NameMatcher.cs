using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing.Matching
{
	public static class IEnumerableExtensions
	{
		public static T ArgMax<T>(this IEnumerable<T> enumerable, Func<T,double> selector)
		{
			T result = default(T);
			bool firstTime = true;
			double best = 0;
			foreach(var e in enumerable)
			{
				var value = selector(e);
				if (firstTime || value>best)
				{
					result = e;
					best = value;
					firstTime = false;
					continue;
				}
			}
			if (firstTime) throw new ArgumentException();
			return result;
		}

	}


	public class NameMatch<TInternal,TExternal>
	{
		double[,] matrix;
		TInternal[] internals;
		TExternal[] externals;
		Func<TInternal, string> internalSelector;
		Func<TExternal, string> externalSelector;
		MatchDataContainer<TInternal, TExternal> result;

		public NameMatch(MatchingPendingData<TInternal,TExternal> pendingData, MatchHandlers<TInternal, TExternal> handlers)
		{
			this.internals = pendingData.Internals.ToArray();
			this.externals = pendingData.Externals.ToArray();
			this.internalSelector = handlers.InternalHandler.Name;
			this.externalSelector = handlers.ExternalHandler.Name;
			result = new MatchDataContainer<TInternal, TExternal>(internals, externals);
		}

		void MakeMatrix()
		{
			matrix=new double[internals.Length,externals.Length];
			for (int i = 0; i < internals.Length; i++)
				for (int j = 0; j < externals.Length; j++)
					matrix[i, j] = LevensteinDistance.RelativeDistance(
						internalSelector(internals[i]),
						externalSelector(externals[j]));
		}

		void MakeMatch(int internalNum, int externalNum)
		{
			result.MakeMatch(internals[internalNum], false, externals[externalNum], false);
			for (int j = 0; j < externals.Length; j++)
				matrix[internalNum, j] = -1;
		}

		void DisableExternal(int externalNum)
		{
			for (int i = 0; i < internals.Length; i++)
				matrix[i, externalNum] = -1;
			result.External[externals[externalNum]] = MatchStatus.Dirty;
		}

		IEnumerable<Tuple<int,int>> PointForInternal(int internalNum)
		{
			for (int e = 0; e < externals.Length; e++)
				yield return Tuple.Create(internalNum, e);
		}

		IEnumerable<Tuple<int,int>> Points
		{
			get
			{
				for (int i = 0; i < internals.Length; i++)
					foreach (var e in PointForInternal(i))
						yield return e;
			}
		}

		const double tooCloseThreshold = 0.8;
		const double matchThreshold = 0.4;
		

		bool MakeIteration()
		{
			var point = Points.ArgMax(z => matrix[z.Item1, z.Item2]);
			var bestValue = matrix[point.Item1,point.Item2];
			if (bestValue<matchThreshold) return false;

			var nextOptimum = PointForInternal(point.Item1)
				.Where(z => z.Item2 != point.Item2)
				.Where(z => matrix[z.Item1, z.Item2] > bestValue * tooCloseThreshold)
				.ToList();

			if (nextOptimum.Count > 0)
			{
				DisableExternal(point.Item2);
				foreach (var e in nextOptimum)
					DisableExternal(point.Item2);
			}
			else
				MakeMatch(point.Item1, point.Item2);
			return true;
		}

		public MatchDataContainer<TInternal,TExternal> Run()
		{
			MakeMatrix();
			while (MakeIteration()) ;
			return result;
		}
	}
}
