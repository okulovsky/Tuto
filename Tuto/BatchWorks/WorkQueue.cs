using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Tuto.Model;

namespace Tuto.BatchWorks
{
    /// <summary>
    /// Interaction logic for BatchWorkWindow.xaml
    /// </summary>
    public partial class WorkQueue
    {
        public WorkQueue(WorkSettings settings)
        {
            Work = new ObservableCollection<BatchWork>();
            WorkSettings = settings;
        }

        private WorkSettings WorkSettings { get; set; }
        private bool wasException { get; set; }
        public ObservableCollection<BatchWork> Work { get; set; }
        private Object addLock = new Object();
        private bool queueWorking { get; set; }
        private int currentIndex { get; set; }
        private Thread queueThread { get; set; }
        private Process currentProcess;
        public Dispatcher Dispatcher { get; set; }

        private void Execute()
        {
            while (currentIndex != this.Work.Count && queueWorking)
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
                    wasException = true;
                    CancelTasksAfterException();
                }
                catch (Exception ex)
                {
                    e.Status = BatchWorkStatus.Failure;
                    e.ExceptionMessage = ex.Message;
                    e.Clean();
                    wasException = true;
                    CancelTasksAfterException();
                };
            }
            queueWorking = false;
            if (!wasException)
            {
                currentIndex = 0;
                Dispatcher.Invoke(this.Work.Clear);
            }
        }

        private void CancelTasksAfterException()
        {
            lock (addLock)
            {
                for (var i = currentIndex + 1; i < this.Work.Count; i++)
                    this.Work[i].Status = BatchWorkStatus.Cancelled;
                currentIndex = this.Work.Count;
            }
        }

        private List<BatchWork> GetFilteredWorks(BatchWork work)
        {
            if (work.Finished() && !work.Forced)
                return new List<BatchWork>();

            var allWorks = new List<BatchWork>();

            foreach (var e in work.BeforeWorks)
            {
                Run(e);
                if (e.Finished()) continue;
                if (WorkSettings.AudioCleanSettings.CurrentOption == Model.Options.Skip && e is CreateCleanSoundWork)
                    continue;
                allWorks.Add(e);
            }
            
            if (!(work is MakeAll))
                allWorks.Add(work);

            foreach (var e in work.AfterWorks)
            {
                if (e.Finished()) continue;
                allWorks.Add(e);
            }

            return allWorks;
        }

        public void Run(IEnumerable<BatchWork> works)
        {
            foreach (var e in works)
                Run(e);
        }

        public void Run(BatchWork work)
        {
            wasException = false;
            var newWork = GetFilteredWorks(work);
            lock (addLock)
            {
                var worksIdentifiers = this.Work
                    .Where(x => x.Status == BatchWorkStatus.Pending || x.Status == BatchWorkStatus.Running)
                    .Select(x => x.Name).ToList(); //for duplicates removing

                foreach (var e in newWork)
                {
                    if (worksIdentifiers.Contains(e.Name))
                        continue;
                    this.Work.Add(e);
                }

                if (!queueWorking && Work.Count != 0)
                {
                    queueThread = new Thread(Execute);
                    queueThread.Start();
                    queueWorking = true;
                }
            }
            
        }

        public void CancelTask(int index)
        {
            var selectedIndex = index == -1 ? currentIndex : index;
            if (selectedIndex > currentIndex)
            {
                this.Work[selectedIndex].Status = BatchWorkStatus.Cancelled;
                return;
            }

            queueThread.Abort();
            queueWorking = false;
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
        }

        private void CleanQueue()
        {
            this.Work.Clear();
            currentIndex = 0;
        }
    }
}