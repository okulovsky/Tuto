using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tuto.Publishing.Matching
{



    public class ManualMatchItem<T>
    {
		public string Name { get; private set; }
        public string Caption { get; private set; }
        public RelayCommand Open { get; private set; }
        public MatchStatus Status { get; set; }
        public Brush Brush 
        {
            get 
            {
				if (Status == MatchStatus.NewMatch) return Brushes.LightBlue;
				else if (Status == MatchStatus.OldMatch) return Brushes.LightGreen;
				else if (Status == MatchStatus.Dirty) return Brushes.LightPink;
				else if (Status == MatchStatus.Pending) return Brushes.White;
                return Brushes.White;
            }
        }
        public T OriginalItem { get; private set; }
        public ManualMatchItem(T item, MatchStatus status, MatchItemHandler<T> handler)
        {
			Name = handler.Name(item);
            Open = new RelayCommand(()=>handler.Open(item));
            Caption=handler.Caption(item);
            OriginalItem=item;
            Status = status;
        }
    }

    public class ManualMatchedPair<TInternal,TExternal>
    {
        public ManualMatchItem<TInternal> Internal { get; private set; }
        public ManualMatchItem<TExternal> External { get; private set; }
        private ManualMatchViewModel<TInternal,TExternal> model;
        public RelayCommand Detach { get; private set; }

		public double Distance { get; private set; }

        public ManualMatchedPair(ManualMatchViewModel<TInternal, TExternal> model, ManualMatchItem<TInternal> _internal, ManualMatchItem<TExternal> _external)
        {
            Internal = _internal;
            External = _external;
            this.model = model;
            Detach = new RelayCommand(() => model.BreakMatch(this));
			Distance = LevensteinDistance.RelativeDistance(Internal.Name, External.Name);

        }
    }




    public class ManualMatchViewModel<TInternal,TExternal>
    {
        public ObservableCollection<ManualMatchItem<TInternal>> UnmatchedInternals { get; private set; }
        public ObservableCollection<ManualMatchItem<TExternal>> UnmatchedExternals { get; private set; }
        public ObservableCollection<ManualMatchedPair<TInternal,TExternal>> Matched { get; private set;}
        public ManualMatchItem<TInternal> SelectedInternal { get; set; }
        public ManualMatchItem<TExternal> SelectedExternal { get; set; }
        public RelayCommand MakeMatchCommand { get; private set; }
		readonly MatchItemHandler<TInternal> InternalHandler;
		readonly MatchItemHandler<TExternal> ExternalHandler;


        public ManualMatchViewModel(MatchHandlers<TInternal, TExternal> handlers)
        {
            this.InternalHandler = handlers.InternalHandler;
            this.ExternalHandler = handlers.ExternalHandler;
            UnmatchedInternals = new ObservableCollection<ManualMatchItem<TInternal>>();
            UnmatchedExternals = new ObservableCollection<ManualMatchItem<TExternal>>();
            Matched = new ObservableCollection<ManualMatchedPair<TInternal, TExternal>>();
            MakeMatchCommand = new RelayCommand(MakeMatch, () => SelectedInternal != null && SelectedExternal != null);
			SortByNameCommand = new RelayCommand(() => Sort(z => z.Internal.Name));
			SortByDistanceCommand = new RelayCommand(() => Sort(z => z.Distance));
			SortByStatusCommand = new RelayCommand(() => Sort(z =>
				(z.Internal.Status == MatchStatus.NewMatch || z.External.Status == MatchStatus.NewMatch) ? 0 : 1));
        }

        void MakeMatch()
        {
            if (SelectedExternal == null || SelectedExternal == null) return;

            SelectedInternal.Status= MatchStatus.NewMatch;
            SelectedExternal.Status= MatchStatus.NewMatch;
            Matched.Insert(0,new ManualMatchedPair<TInternal, TExternal>(this, SelectedInternal, SelectedExternal));
			UnmatchedInternals.Remove(SelectedInternal);
			UnmatchedExternals.Remove(SelectedExternal);
        }

        public void BreakMatch(ManualMatchedPair<TInternal, TExternal> pair)
        {
            Matched.Remove(pair);
			pair.Internal.Status = MatchStatus.Dirty;
			pair.External.Status = MatchStatus.Dirty;
            UnmatchedInternals.Add(pair.Internal);
            UnmatchedExternals.Add(pair.External);
        }


        public void AddInternalUnmatched(TInternal _internal, MatchStatus status)
        {
            UnmatchedInternals.Add(new ManualMatchItem<TInternal>(_internal, status, InternalHandler));
        }

        public void AddExternalUnmatched(TExternal _external, MatchStatus status)
        {
            UnmatchedExternals.Add(new ManualMatchItem<TExternal>(_external, status, ExternalHandler));
        }

        public void AddMatch(TInternal _internal, MatchStatus internalStatus, TExternal _external, MatchStatus externalStatus)
        {
            Matched.Add(new ManualMatchedPair<TInternal, TExternal>(
                this,
                new ManualMatchItem<TInternal>(_internal, internalStatus, InternalHandler),
                new ManualMatchItem<TExternal>(_external, externalStatus, ExternalHandler)
                ));
        }

		public void Pull(MatchDataContainer<TInternal, TExternal> dataContainer)
		{
			foreach (var e in dataContainer.Match)
				AddMatch(e.Key, dataContainer.Internal[e.Key], e.Value, dataContainer.External[e.Value]);
			foreach (var e in dataContainer.Internal.Where(z => z.Value == MatchStatus.Dirty))
				AddInternalUnmatched(e.Key, e.Value);
			foreach (var e in dataContainer.External.Where(z => z.Value == MatchStatus.Dirty))
				AddExternalUnmatched(e.Key, e.Value);
		}

		public void Pull(MatchingPendingData<TInternal,TExternal> pendingData)
		{
			foreach (var e in pendingData.Internals) AddInternalUnmatched(e, MatchStatus.Pending);
			foreach (var e in pendingData.Externals) AddExternalUnmatched(e, MatchStatus.Pending);
		}

		#region Sorting
		void Sort<T>(Func<ManualMatchedPair<TInternal,TExternal>,T> sortKey)
		{
			var list = Matched.OrderBy(sortKey).ToList();
			Matched.Clear();
			foreach (var e in list) Matched.Add(e);
		}

		public RelayCommand SortByNameCommand { get; private set; }
		public RelayCommand SortByStatusCommand { get; private set; }
		public RelayCommand SortByDistanceCommand { get; private set; }

		public void Prepare()
		{
			Sort(z => z.Internal.Name);
		}
		#endregion

		public void Push(MatchUpdater<TInternal,TExternal> updater)
		{
			foreach (var e in UnmatchedInternals)
				if (e.Status != MatchStatus.Pending)
					updater.ClearInternal(e.OriginalItem);
			foreach (var e in UnmatchedExternals)
				if (e.Status != MatchStatus.Pending)
					updater.ClearExternal(e.OriginalItem);
			foreach (var pair in Matched)
			{
				if (pair.Internal.Status != MatchStatus.OldMatch)
					updater.InternalTakesExternal(pair.Internal.OriginalItem, pair.External.OriginalItem);
				if (pair.External.Status != MatchStatus.OldMatch)
					updater.ExternalTakesInternal(pair.External.OriginalItem, pair.Internal.OriginalItem);
			}
		}
	}
}
