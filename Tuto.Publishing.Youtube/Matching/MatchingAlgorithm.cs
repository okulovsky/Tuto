using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing.Matching
{
	public static class MatchingAlgorithm
	{
		public static void Run<TInternal, TExternal, TInternalKey, TExternalKey>(
			IEnumerable<TInternal> internals,
			IEnumerable<TExternal> externals,
			MatchHandlersAndKeys<TInternal, TExternal, TInternalKey, TExternalKey> handlers)
			where TInternal : class
			where TExternal : class
		{
			var manualMatchViewModel = new ManualMatchViewModel<TInternal, TExternal>(handlers.Handlers);
			var pendingData = new MatchingPendingData<TInternal,TExternal>(internals,externals);

			var idMatch = new TwoDirectionalIdMatch<TInternal, TExternal, TInternalKey, TExternalKey>(pendingData, handlers.KeySet);
			var idMatchResult = idMatch.Run();
			manualMatchViewModel.Pull(idMatchResult);
			pendingData = idMatchResult.GetPendingData();

			var nameMatch = new NameMatch<TInternal, TExternal>(pendingData, handlers.Handlers);
			var nameMatchResult = nameMatch.Run();
			manualMatchViewModel.Pull(nameMatchResult);
			pendingData = nameMatchResult.GetPendingData();

			foreach (var e in pendingData.Internals) manualMatchViewModel.AddInternalUnmatched(e, MatchStatus.Pending);
			foreach (var e in pendingData.Externals) manualMatchViewModel.AddExternalUnmatched(e, MatchStatus.Pending);

			var window = new Tuto.Publishing.Views.ManualMatch();
			window.DataContext = manualMatchViewModel;
			window.ShowDialog();

			if (window.DialogResult==true)
			{
				foreach (var e in manualMatchViewModel.UnmatchedInternals)
					if (e.Status!= MatchStatus.Pending)
						handlers.Updaters.ClearInternal(e.OriginalItem);
				foreach (var e in manualMatchViewModel.UnmatchedExternals)
					if (e.Status!= MatchStatus.Pending)
						handlers.Updaters.ClearExternal(e.OriginalItem);
				foreach (var pair in manualMatchViewModel.Matched)
				{
					if (pair.Internal.Status!= MatchStatus.OldMatch)
						handlers.Updaters.InternalTakesExternal(pair.Internal.OriginalItem, pair.External.OriginalItem);
					if (pair.External.Status!= MatchStatus.OldMatch)
					handlers.Updaters.ExternalTakesInternal(pair.External.OriginalItem, pair.Internal.OriginalItem);
				}
			}
			
		}
	}
}
