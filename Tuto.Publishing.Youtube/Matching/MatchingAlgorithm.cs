using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing.Matching
{
	public static class MatchingAlgorithm
	{
		public static void RunWeakAlgorithm<TInternal,TExternal>(
			IEnumerable<TInternal> internals, 
			IEnumerable<TExternal> externals,
			MatchHandlers<TInternal,TExternal> handlers,
			MatchUpdater<TInternal,TExternal> updates)
		{
			var pendingData = new MatchingPendingData<TInternal, TExternal>(internals, externals);
			var nameMatch = new NameMatch<TInternal, TExternal>(pendingData, handlers);
			var nameMatchResult = nameMatch.Run();
			var model = new ManualMatchViewModel<TInternal, TExternal>(handlers);
			model.Pull(nameMatchResult);
			model.Pull(nameMatchResult.GetPendingData());
			model.Push(updates);
		}


		public static void RunStrongAlgorithm<TInternal, TExternal, TInternalKey, TExternalKey>(
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

			manualMatchViewModel.Pull(pendingData);
		
			manualMatchViewModel.Prepare();
			var window = new Tuto.Publishing.Views.ManualMatch();
			window.DataContext = manualMatchViewModel;
			window.ShowDialog();

			if (window.DialogResult==true)
			{
				manualMatchViewModel.Push(handlers.Updaters);
			}
		}
	}
}
