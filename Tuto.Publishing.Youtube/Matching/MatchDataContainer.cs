using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing.Matching
{
	public enum MatchStatus
	{
		//Item was not matched and is waiting for it
		Pending,
		//Item was already matched 
		OldMatch,
		//Item was just now matched
		NewMatch,
		//Item is broken, and should be matched manually
		Dirty,
		//Item should be excluded from matching permanently
		Denied
	}

	public class MatchDataContainer<TInternal,TExternal>
	{
		public readonly Dictionary<TInternal, MatchStatus> Internal = new Dictionary<TInternal, MatchStatus>();
		public readonly Dictionary<TExternal, MatchStatus> External = new Dictionary<TExternal, MatchStatus>();
		public readonly Dictionary<TInternal, TExternal> Match = new Dictionary<TInternal, TExternal>();

		public MatchDataContainer(IEnumerable<TInternal> _internal, IEnumerable<TExternal> _external)
		{
			foreach (var e in _internal) Internal[e] = MatchStatus.Pending;
			foreach (var e in _external) External[e] = MatchStatus.Pending;
		}
		public void MakeMatch(TInternal _internal, bool internalOldMatch, TExternal _external, bool externalOldMatch)
		{
			Match[_internal] = _external;
			Internal[_internal] = internalOldMatch ? MatchStatus.OldMatch : MatchStatus.NewMatch;
			External[_external] = externalOldMatch ? MatchStatus.OldMatch : MatchStatus.NewMatch;
		}



		public MatchStatus GetStatus(object obj)
		{
			if (obj is TInternal)
			{
				var i = (TInternal)obj;
				if (Internal.ContainsKey(i)) return Internal[i];
			}
			if (obj is TExternal)
			{
				var e = (TExternal)obj;
				if (External.ContainsKey(e)) return External[e];
			}
			throw new ArgumentException();
		}

		public void SetStatus(object obj, MatchStatus status)
		{
			if (obj is TInternal)
			{
				var i = (TInternal)obj;
				if (Internal.ContainsKey(i))
				{
					Internal[i] = status;
					return;
				}
			}
			if (obj is TExternal)
			{
				var e = (TExternal)obj;
				if (External.ContainsKey(e))
				{
					External[e] = status;
					return;
				}
			}
			throw new ArgumentException();
		}

	}
}
