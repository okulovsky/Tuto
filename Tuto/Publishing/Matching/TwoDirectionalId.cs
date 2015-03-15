using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing.Matching
{
	
	public partial class TwoDirectionalIdMatch<TInternal, TExternal, TInternalKey, TExternalKey>
		where TInternalKey : class
		where TExternalKey : class
		where TInternal : class
		where TExternal : class
		
	{

		//input data
		readonly IEnumerable<TInternal> Internal;
		readonly IEnumerable<TExternal> External;
		readonly Func<TInternal, TInternalKey> intToInt;
		readonly Func<TInternal, TExternalKey> intToExt;
		readonly Func<TExternal, TInternalKey> extToInt;
		readonly Func<TExternal, TExternalKey> extToExt;

	

		//result
		readonly DataContainer<TInternal, TExternal> result;

		public TwoDirectionalIdMatch(
			IEnumerable<TInternal> Internal,
			IEnumerable<TExternal> External,
			Func<TInternal, TInternalKey> intToInt,
			Func<TInternal, TExternalKey> intToExt,
			Func<TExternal, TInternalKey> extToInt,
			Func<TExternal, TExternalKey> extToExt)
		{
			this.Internal=Internal;
			this.External=External;
			this.intToInt=intToInt;
			this.intToExt=intToExt;
			this.extToInt=extToInt;
			this.extToExt=extToExt;
			result = new DataContainer<TInternal, TExternal>(Internal, External);
		}

		Dictionary<object, object> map;


		void BuildMapAndClearLinks()
		{
			map = new Dictionary<object, object>();
			
			var intIds = Internal.ToDictionary(z=>intToInt(z),z=>z);
			var extIds = External.ToDictionary(z=>extToExt(z),z=>z);

			
			foreach(var i in Internal)
			{
				map[i] = null;
				var eKey = intToExt(i);
				if (eKey == null) continue;
				if (!extIds.ContainsKey(eKey))
				{
					result.SetStatus(i, MatchStatus.Dirty);
					continue;
				}
				map[i] = extIds[eKey];
			}

			foreach (var e in External)
			{
				map[e] = null;
				var iKey = extToInt(e);
				if (iKey == null) continue;
				if (!intIds.ContainsKey(iKey))
				{
					result.SetStatus(e, MatchStatus.Denied);
					continue;
				}
				map[e] = intIds[iKey];
			}
		}

		void MarkPrimaryDirtyLinks()
		{
			foreach(var from in map.Keys)
			{
				var to = map[from];
				if (to == null) continue;
				if (result.GetStatus(to)!= MatchStatus.Pending)
				{
					result.SetStatus(from, MatchStatus.Dirty);
					continue;
				}
				var ret = map[to];
				if (ret == null) continue;
				if (ret != from)
					result.SetStatus(from, MatchStatus.Dirty);
			}
		}

		void MarkSecondaryDirtyLinks()
		{
			var queue = new Queue();
			foreach(var item in map.Keys)
				if (result.GetStatus(item) != MatchStatus.Pending) 
					queue.Enqueue(item);
			var visited = new HashSet<object>();
			while(queue.Count!=0)
			{
				var item = queue.Dequeue();
				if (visited.Contains(item)) continue;
				visited.Add(item);
				if (map[item] != null)
				{
					result.SetStatus(map[item], MatchStatus.Dirty);
					queue.Enqueue(map[item]);
				}
				foreach (var from in map.Keys)
					if (map[from] == item)
					{
						result.SetStatus(from, MatchStatus.Dirty);
						queue.Enqueue(from);
					}
			}
		}

		void MakeMatch()
		{
			foreach(var e in map.Keys)
			{
				if (result.GetStatus(e) != MatchStatus.Pending) continue;
				var to = map[e];
				if (to == null) continue;
				var from = map[to];
				if (Internal.Contains(e))
					result.MakeMatch((TInternal)e, true, (TExternal)to, from!=null);
				else
					result.MakeMatch((TInternal)to, from!=null, (TExternal)e, true);
			}
		}




		public DataContainer<TInternal, TExternal> Run()
		{
			BuildMapAndClearLinks();
			MarkPrimaryDirtyLinks();
			MarkSecondaryDirtyLinks();
			MakeMatch();
			return result;
		}
	}
}
