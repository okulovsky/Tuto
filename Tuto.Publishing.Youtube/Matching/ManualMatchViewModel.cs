using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tuto.Publishing.Matching
{

    public class ManualMatchItemHandler<T>
    {
        public Func<T, string> Caption { get; set; }
        public Action<T> Open { get; set; }
    }

    public class ManualMatchItem<T>
    {
        public string Caption { get; private set; }
        public RelayCommand Open { get; private set; }
        public MatchStatus Status { get; set; }
        public Brush Brush 
        {
            get 
            {
                if (Status == MatchStatus.NewMatch) return Brushes.LightPink;
                else if (Status == MatchStatus.OldMatch) return Brushes.LightGreen;
                return Brushes.White;
            }
        }
        public T OriginalItem { get; private set; }
        public ManualMatchItem(T item, MatchStatus status, ManualMatchItemHandler<T> handler)
        {
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

        public ManualMatchedPair(ManualMatchViewModel<TInternal, TExternal> model, ManualMatchItem<TInternal> _internal, ManualMatchItem<TExternal> _external)
        {
            Internal = _internal;
            External = _external;
            this.model = model;
            Detach = new RelayCommand(() => model.BreakMatch(this));

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
        ManualMatchItemHandler<TInternal> InternalHandler { get; private set; }
        ManualMatchItemHandler<TExternal> ExternalHandler { get; private set; }


        public ManualMatchViewModel(ManualMatchItemHandler<TInternal> InternalHandler, ManualMatchItemHandler<TExternal> ExternalHandler)
        {
            this.InternalHandler = InternalHandler;
            this.ExternalHandler = ExternalHandler;
            UnmatchedInternals = new ObservableCollection<ManualMatchItem<TInternal>>();
            UnmatchedExternals = new ObservableCollection<ManualMatchItem<TExternal>>();
            Matched = new ObservableCollection<ManualMatchedPair<TInternal, TExternal>>();
            MakeMatchCommand = new RelayCommand(MakeMatch, () => SelectedInternal != null && SelectedExternal != null);
        }

        void MakeMatch()
        {
            if (SelectedExternal == null || SelectedExternal == null) return;
            UnmatchedInternals.Remove(SelectedInternal);
            UnmatchedExternals.Remove(SelectedExternal);
            SelectedInternal.Status= MatchStatus.NewMatch;
            SelectedExternal.Status= MatchStatus.NewMatch;
            Matched.Add(new ManualMatchedPair<TInternal,TExternal>(this,SelectedInternal,SelectedExternal);
        }

        public void BreakMatch(ManualMatchedPair<TInternal, TExternal> pair)
        {
            Matched.Remove(pair);
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
    }
}
