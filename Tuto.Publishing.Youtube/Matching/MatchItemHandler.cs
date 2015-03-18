using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing.Matching
{
	public class MatchingPendingData<TInternal,TExternal>
	{
		public readonly IEnumerable<TInternal> Internals;
		public readonly IEnumerable<TExternal> Externals;
		public MatchingPendingData(IEnumerable<TInternal> internals, IEnumerable<TExternal> externals)
		{
			Internals = internals;
			Externals = externals;
		}
	}


	public class MatchItemHandler<TItem>
	{
		/// <summary>
		/// Only name of the TItem for matching
		/// </summary>
		public readonly Func<TItem, string> Name;
		/// <summary>
		/// Caption can contain name and other data, it is not used for matching
		/// </summary>
		public readonly Func<TItem, string> Caption;

		public readonly Action<TItem> Open;
		
		public MatchItemHandler(Func<TItem, string> name, Func<TItem, string> caption, Action<TItem> open)
		{
			Name=name;
			Caption=caption;
			Open=open;
		}
	}

	public class MatchKeySet<TInternal, TExternal, TInternalKey, TExternalKey>
	{
		public readonly Func<TInternal, TInternalKey> IntToInt;
		public readonly Func<TInternal, TExternalKey> IntToExt;
		public readonly Func<TExternal, TInternalKey> ExtToInt;
		public readonly Func<TExternal, TExternalKey> ExtToExt;
		public readonly Func<TInternalKey, bool> EmptyInternal;
		public readonly Func<TExternalKey, bool> EmptyExternal;



		public MatchKeySet(
			Func<TInternal, TInternalKey> intToInt,
			Func<TInternal, TExternalKey> intToExt,
			Func<TExternal, TInternalKey> extToInt,
			Func<TExternal, TExternalKey> extToExt,
			Func<TInternalKey, bool> emptyInternal,
			Func<TExternalKey, bool> emptyExternal)
		{
			IntToInt=intToInt;
			IntToExt=intToExt;
			ExtToInt=extToInt;
			ExtToExt=extToExt;
			EmptyExternal = emptyExternal;
			EmptyInternal = emptyInternal;
		}
	}

	public class MatchHandlers<TInternal, TExternal>
	{
		public readonly MatchItemHandler<TInternal> InternalHandler;
		public readonly MatchItemHandler<TExternal> ExternalHandler;
		public MatchHandlers(MatchItemHandler<TInternal> internalHandler, MatchItemHandler<TExternal> externalHandler)
		{
			InternalHandler = internalHandler;
			ExternalHandler = externalHandler;
		}
	}

	public class MatchHandlersAndKeys<TInternal, TExternal, TInternalKey, TExternalKey>
	{
		public readonly MatchHandlers<TInternal, TExternal> Handlers;
		public readonly MatchKeySet<TInternal, TExternal, TInternalKey, TExternalKey> KeySet;
		public MatchHandlersAndKeys(MatchHandlers<TInternal, TExternal> handlers, MatchKeySet<TInternal, TExternal, TInternalKey, TExternalKey> keySet )
		{
			Handlers = handlers;
			KeySet = keySet;
		}
	}
}
