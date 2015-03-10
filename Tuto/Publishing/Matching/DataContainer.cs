using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
	public enum MatchStatus
	{
		//Item was not matched and is waiting for it
		Pending,
		//Item was matched
		Matched,
		//Item was not matched, and should not be matched later
		Denied
	}

	public class DataContainer<TInternal,TExternal>
	{
		public readonly Dictionary<TInternal, MatchStatus> Internal = new Dictionary<TInternal, MatchStatus>();
		public readonly Dictionary<TExternal, MatchStatus> External = new Dictionary<TExternal, MatchStatus>();
		public readonly Dictionary<TInternal, TExternal> Match = new Dictionary<TInternal, TExternal>();

		public DataContainer(IEnumerable<TInternal> _internal, IEnumerable<TExternal> _external)
		{
			foreach (var e in _internal) Internal[e] = MatchStatus.Pending;
			foreach (var e in _external) External[e] = MatchStatus.Pending;
		}
		public void MakeMatch(TInternal _internal, TExternal _external)
		{
			Match[_internal] = _external;
			Internal[_internal] = MatchStatus.Matched;
			External[_external] = MatchStatus.Matched;
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

		public void Deny(TInternal _internal)
		{
			Internal[_internal] = MatchStatus.Denied;
		}
		public void Deny(TExternal _external)
		{
			External[_external] = MatchStatus.Denied;
		}
	}
}
