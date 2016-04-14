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

    public class WorkQueue
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


        bool ModelInQueue(EditorModel model)
        {
            return Work.Any(z => z.Model == model && z.Status == BatchWorkStatus.Pending || z.Status == BatchWorkStatus.Running);
        }

        private void Execute()
        {
            while (currentIndex < this.Work.Count && queueWorking)
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
                    if (e is ProcessBatchWork)
                        currentProcess = (e as ProcessBatchWork).Process;

                    var task = GetNextTask(e);
                    e.Work();
                    while (task != e)
                    {
                        if (!(task is CompositeWork))
                        {
                            task.Status = BatchWorkStatus.Running;
                            task.Work();
                            task.Status = BatchWorkStatus.Success;
                        }
                        else
                        {
                            task = GetNextTask(e);
                        }
                    }
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
				if (e.Model!=null)
					e.Model.Statuses.InQueue = ModelInQueue(e.Model);
            }
            queueWorking = false;
            if (!wasException)
            {
                currentIndex = 0;
                Dispatcher.Invoke(this.Work.Clear);
            }
        }

        private BatchWork GetNextTask(BatchWork work)
        {
            if (work.ChildWorks != null)
                foreach (var e in work.ChildWorks)
                    if (e.Status == BatchWorkStatus.Pending)
                        return GetNextTask(e);
            return work;
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

        private void FilteredAdd(List<BatchWork> allWorks, BatchWork work)
        {
            if (!(work is CompositeWork))
                    allWorks.Add(work);
        }

        private List<BatchWork> GetFilteredWorks(BatchWork work)
        {
            if (work.Finished() && !work.Forced)
                return new List<BatchWork>();

            var allWorks = new List<BatchWork>();

            if (work is CompositeWork)
            {
                foreach (var e in (work as CompositeWork).Tasks)
                {
                    if (e.Finished()) continue;
                    if (WorkSettings.AudioCleanSettings.CurrentOption == Options.Skip && e is CreateCleanSoundWork)
                        continue;
                    if (!WorkSettings.AutoUploadVideo && e is YoutubeWork)
                        continue; 
                    Run(e);
                    FilteredAdd(allWorks, e);
                }
            }

            FilteredAdd(allWorks, work);
            return allWorks;
        }

        public void Run(IEnumerable<BatchWork> works)
        {
            foreach (var e in works)
                Run(e);
        }

        public void NewRun(BatchWork work)
        {
            if (work is CompositeWork)
            {
                var compWork = work as CompositeWork;
                ///insert new node and put in it
                //Work.Add(work);
                AddNode(work, null);
            }
        }

        public void AddNode(BatchWork work, BatchWork parent)
        {
            if (work is CompositeWork)
            {
                var compWork = work as CompositeWork;
                if (parent != null)
                {
                    if (parent.ChildWorks == null) parent.ChildWorks = new ObservableCollection<BatchWork>();
                    parent.ChildWorks.Add(work);
                    work.Parent = parent;
                }
                foreach (var e in compWork.Tasks)
                {
                    AddNode(e, work);
                }
            }
            else
            {
                if (parent.ChildWorks == null)
                    parent.ChildWorks = new ObservableCollection<BatchWork>();
                parent.ChildWorks.Add(work);
                work.Parent = parent;
            }
        }

        public void Run(BatchWork work)
        {
            wasException = false;
            NewRun(work);
            Work.Add(work);

            if (!queueWorking && Work.Count != 0)
            {
                queueThread = new Thread(Execute);
                queueThread.Start();
                queueWorking = true;
            }

            //       var worksIdentifiers = this.Work
            //               .Where(x => x.Status == BatchWorkStatus.Pending || x.Status == BatchWorkStatus.Running)
            //               .ToList(); //for duplicates removing

            //       var newWork = GetFilteredWorks(work);
            //       lock (addLock)
            //       {
            //           foreach (var e in newWork)
            //           {
            //               if (worksIdentifiers.Count(x => x.Model == e.Model && e.GetType() == x.GetType()) != 0)
            //                   continue;
            //               Work.Add(e);
            //if (e.Model!=null)
            //	e.Model.Statuses.InQueue = true;
            //           }


            //       }

        }

        public void RemoveOldTasks()
        {
            for (int i = 0; i < Work.Count; i++)
                if (Work[i].Status != BatchWorkStatus.Pending && Work[i].Status != BatchWorkStatus.Running)
                {
                    Work.RemoveAt(i);
                    i--;
                }
        }

        public void CancelTask(int index)
        {
            var selectedIndex = index == -1 ? currentIndex : index;
            if (selectedIndex > currentIndex)
            {
                Work[selectedIndex].Status = BatchWorkStatus.Cancelled;
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
                    Work[i].Status = BatchWorkStatus.Aborted;
                }
                else
                    Work[i].Status = BatchWorkStatus.Cancelled;
            }
        }

        private void CleanQueue()
        {
            Work.Clear();
            currentIndex = 0;
        }
    }
}