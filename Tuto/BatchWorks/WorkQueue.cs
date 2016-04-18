using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private bool wasWorkAborted;
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
                if (e.Status == BatchWorkStatus.Cancelled || e.Status == BatchWorkStatus.Aborted)
                {
                    currentIndex++;
                    continue;
                }
                e.Status = BatchWorkStatus.Running;
                try
                {
                    var task = GetNextTask(e);
                    if (task == e) //for atomic work
                    {
                        e.Work();
                        e.Status = BatchWorkStatus.Success;
                    }

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
                            if (task.ChildWorks.Count(x => x.Status == BatchWorkStatus.Success) == task.ChildWorks.Count)
                                task.Status = BatchWorkStatus.Success;
                        }

                        task = GetNextTask(e);
                    }
                    e.Status = BatchWorkStatus.Success;
                    currentIndex++;
                }
                catch (Exception ex)
                {
                    if (!wasWorkAborted)
                    {
                        e.Status = BatchWorkStatus.Failure;
                        e.ExceptionMessage = ex.Message;
                        e.Clean();
                        wasException = true;
                    }
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
                    if (e.Status == BatchWorkStatus.Pending || e.Status == BatchWorkStatus.Running)
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

        private bool ShouldWeDoThisWork(BatchWork work) // filterer
        {
            
            if (work.Finished() && !work.Forced)
                return false;
            if (work.Finished()) return false;
            if (WorkSettings.AudioCleanSettings.CurrentOption == Options.Skip && work is CreateCleanSoundWork)
                return false;
            if (!WorkSettings.AutoUploadVideo && work is YoutubeWork)
                return false;
            return true;
        }

        public void Run(IEnumerable<BatchWork> works)
        {
            foreach (var e in works)
                Run(e);
        }

        public void UnpackWork(BatchWork work)
        {
            if (work is CompositeWork)
            {
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
                if (ShouldWeDoThisWork(work))
                {
                    parent.ChildWorks.Add(work);
                    work.Parent = parent;
                }
            }
        }

        public void Run(BatchWork work)
        {
            wasException = false;
            var worksInQueue = this.Work
                    .Where(x => x.Status == BatchWorkStatus.Pending || x.Status == BatchWorkStatus.Running)
                    .ToList(); //for duplicates removing
            if (worksInQueue.Any(x => x.Model == work.Model && x.GetType() == work.GetType()))
                return; //reject work as duplicate at top level

            UnpackWork(work); //prepare tree in work

            lock (addLock)
            {
                Work.Add(work);
            }

            if (!queueWorking && Work.Count != 0)
            {
                queueThread = new Thread(Execute);
                queueThread.Start();
                queueWorking = true;
            }
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

        public void CancelTask(BatchWork work)
        {
            if (work is CompositeWork)
            {
                work.Status = BatchWorkStatus.Cancelled;
                CancelCompositeWork(work);
            }
            else
            {
                CancelAtomicWork(work);
            }   
        }

        private void AbortTask(BatchWork work)
        {
            wasWorkAborted = true;
            queueThread.Abort();
            work.Clean();
            queueThread = new Thread(Execute);
            queueThread.Start();
            work.Status = BatchWorkStatus.Aborted;
            wasWorkAborted = false;
        }

        private void CancelAtomicWork(BatchWork work)
        {
            if (work.Parent != null)
                foreach (var e in work.Parent.ChildWorks)
                {
                    e.Parent.Status = BatchWorkStatus.Cancelled;
                    if (e.Status == BatchWorkStatus.Running)
                    {
                        AbortTask(e);
                    }
                    else
                        e.Status = BatchWorkStatus.Cancelled;
                }

            if (work.Status == BatchWorkStatus.Running)
            {
                AbortTask(work);
            }
            else if (work.Status == BatchWorkStatus.Pending)
                work.Status = BatchWorkStatus.Cancelled;
        }

        private void CancelCompositeWork(BatchWork work)
        {
            if (work.ChildWorks != null)
                foreach (var e in work.ChildWorks)
                {
                    if (e.Status == BatchWorkStatus.Running)
                        AbortTask(e);
                    else
                        e.Status = BatchWorkStatus.Cancelled;
                    CancelCompositeWork(e);
                } 
        }

        private void CleanQueue()
        {
            Work.Clear();
            currentIndex = 0;
        }
    }
}