using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tuto.BatchWorks;

namespace Tuto.Navigator.NewLook
{
    public class WorkQueueViewModel : NotifierModel
    {
        WorkQueue queue;

        public int TasksCount { get { return queue.Work.Count(z => z.Status == BatchWorkStatus.Pending); } }
        public int FailuresCount { get { return queue.Work.Count(z => z.Status == BatchWorkStatus.Failure); } }

        public ObservableCollection<BatchWork> Works { get { return queue.Work;  } }
        public Visibility FailuresVisible { get { return FailuresCount > 0 ? Visibility.Visible : Visibility.Collapsed; } }
        public WorkQueueViewModel(WorkQueue queue)
        {
            this.queue = queue;
            
            queue.Work.CollectionChanged += (s, a) => NotifyAll();
            queue.StatusChanged += w => NotifyAll();
        }
    }
}
