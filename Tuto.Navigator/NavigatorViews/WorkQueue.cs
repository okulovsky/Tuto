using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Tuto.BatchWorks;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Tuto.Navigator
{
    /// <summary>
    /// Interaction logic for BatchWorkWindow.xaml
    /// </summary>
    public partial class WorkQueue
    {
        public WorkQueue(VideothequeModel vdModel)
        {
            VDModel = vdModel;
            Work = new ObservableCollection<BatchWork>(); 
        }

        private VideothequeModel VDModel;
        public ObservableCollection<BatchWork> Work { get; set; }
        private Object addLock = new Object();
        private bool QueueWorking { get; set; }
        private int currentIndex { get; set; }
        private Thread queueThread { get; set; }
        private Process currentProcess;
        public Dispatcher Dispatcher { get; set; }

        void Execute()
        {
            while (currentIndex != this.Work.Count && QueueWorking)
            {
                BatchWork e = Work[currentIndex];
                if (e.Status == BatchWorkStatus.Cancelled)
                {
                    currentIndex++;
                    continue;
                }
                e.Status = BatchWorkStatus.Running;
                try
                {
                    currentProcess = e.Process;
                    e.Work();
                    e.Status = BatchWorkStatus.Success;
                    currentIndex++;
                }
                catch (ThreadAbortException)
                {
                    e.Status = BatchWorkStatus.Aborted;
                    e.Clean();
                    currentIndex++;
                }
                catch (Exception ex)
                {
                    e.Status = BatchWorkStatus.Failure;
                    e.ExceptionMessage = ex.Message;
                    e.Clean();
                    currentIndex++;
                };
            }
            QueueWorking = false;
            currentIndex = 0;
            Dispatcher.Invoke(this.Work.Clear);
        }

        public List<BatchWork> ApplyFilter(BatchWork work)
        {
            var allWorks = new List<BatchWork>();
            foreach (var e in work.BeforeWorks)
            {
                //filter
                allWorks.Add(e);
            }
            allWorks.Add(work);
            return allWorks;
        }

        public void Run(IEnumerable<BatchWork> works)
        {
            foreach (var e in works)
                Run(e);
        }

        public void Run(BatchWork work)
        {
            var newWork = ApplyFilter(work);
            lock (addLock)
            {
                foreach (var e in newWork)
                    this.Work.Add(e);

            }
            if (this.Work.Count == 0)
                return;

            if (!QueueWorking)
            {

                queueThread = new Thread(Execute);
                queueThread.Start();
            }
            QueueWorking = true;
        }

        public void CancelTask(int index)
        {
            var selectedIndex = index == -1 ? 0 : index;
            if (selectedIndex != 0)
            {
                this.Work[selectedIndex].Status = BatchWorkStatus.Cancelled;
                return;
            }

            queueThread.Abort();
            QueueWorking = false;
            if (currentProcess != null && !currentProcess.HasExited)
            {
                currentProcess.Kill();
            }
            for (var i = currentIndex; i < this.Work.Count; i++)
            {
                if (i == currentIndex)
                {
                    this.Work[i].Status = BatchWorkStatus.Aborted;
                }
                else
                    this.Work[i].Status = BatchWorkStatus.Cancelled;
            }
            CleanQueue();
        }

        private void CleanQueue()
        {
            this.Work.Clear();
            currentIndex = 0;
        }
    }
}