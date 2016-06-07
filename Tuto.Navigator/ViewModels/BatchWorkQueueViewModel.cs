using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tuto.BatchWorks;

namespace Tuto.Navigator.ViewModels
{
    public class BatchWorkQueueViewModel : NotifierModel
    {
        public readonly WorkQueue queue;

        public ObservableCollection<BatchWork> Works { get { return queue.Work; } }
        public BatchWork SelectedWork { get; set; }

        public int TotalWorks { get; private set; }
        public int CompletedWorks { get; private set; }

        public Visibility ErrorVisible { get; private set; }

        public RelayCommand Cancel { get; private set; }

        public RelayCommand Clear { get; private set; }

        public BatchWorkQueueViewModel(WorkQueue queue)
        {
            this.queue = queue;
            queue.Work.CollectionChanged += Work_CollectionChanged;
            Cancel = new RelayCommand(() => queue.CancelTask(SelectedWork), () => TotalWorks-CompletedWorks != 0);
            Clear = new RelayCommand(() => queue.RemoveOldTasks());
            UpdateData();
        }

        void Work_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems!=null)
                foreach (var x in e.NewItems)
                    ((BatchWork)x).SubsrcibeByExpression(z => z.Status, UpdateData);
            UpdateData();
        }

        void UpdateData()
        {
            TotalWorks = queue.Work.Count();
            CompletedWorks = queue.Work.Where(z => z.Status == BatchWorkStatus.Success).Count();
            ErrorVisible = queue.Work.Where(z => z.Status != BatchWorkStatus.Success && z.Status!= BatchWorkStatus.Pending && z.Status!= BatchWorkStatus.Running).Any() ? Visibility.Visible : Visibility.Collapsed;
            base.NotifyAll();
        }

    }
}
