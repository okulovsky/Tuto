using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.IO;
using Tuto.Model;
using System.ComponentModel;
using System.Threading;
using System.Collections.ObjectModel;

namespace Tuto.BatchWorks
{
    public enum BatchWorkStatus
    {
        Pending,
        Running,
        Success,
        Failure,
        Aborted,
        Cancelled,
        Attention
    }

    public abstract class BatchWork : NotifierModel
    {
        public virtual EditorModel Model { get; set; }
        public bool NeedToRewrite { get; set; }
        public ObservableCollection<BatchWork> ChildWorks { get; set; }
        public BatchWork Parent { get; set; }

        public RelayCommand CopyCmd { get; set; }

        public BatchWork()
        {
            CopyCmd = new RelayCommand(CmCopyCmd);
        }

        void CmCopyCmd()
        {
            Clipboard.SetText(exceptionMessage);
        }

        public IEnumerable<BatchWork> WorkTree
        {
            get
            {
                yield return this;
                if (ChildWorks!=null)
                    foreach(var e in ChildWorks)
                        yield return e;
            }
        }


        public virtual void Work() { }
        public virtual void Clean() { }
        public virtual bool WorkIsRequired() { return true; }
        

        public FileInfo GetTempFile(FileInfo info, string suffix)
        {
            var newPath = info.FullName.Split('\\');
            var nameAndExt = info.Name.Split('.');
            nameAndExt[0] = nameAndExt[0] + suffix;
            newPath[newPath.Length - 1] = string.Join(".", nameAndExt);
            return new FileInfo(string.Join("\\", newPath));
        }

        public FileInfo GetTempFile(FileInfo info)
        {
            return GetTempFile(info, "-tmp");
        }

        public virtual bool Finished() { return false; }
        public bool Forced { get; set; }

        public string Name { get; set; }

        private double progress;
        public double Progress
        {
            get { return progress; }
            set
            {
                var delta = value - progress;
                progress = Math.Min(100, Math.Max(0,value));
                NotifyPropertyChanged();
                if (Parent != null)
                {
                    var tasksCount = Parent.ChildWorks.Count;
                    Parent.Progress = Parent.Progress + delta / tasksCount;
                }
            }
        }

        BatchWorkStatus status;
        public BatchWorkStatus Status
        {
            get { return status; }
            set
            {
                if (Parent != null && (value == BatchWorkStatus.Running || value == BatchWorkStatus.Failure) && Parent.Status != value )
                    Parent.Status = value; //Running and failure means status for all Parents too
                status = value;
                NotifyPropertyChanged();
            }
        }

        public void TryToDelete(FileInfo info)
        {
            TryToDelete(info.FullName);
        }

        public void TryToDelete(string fileName)
        {
            var tries = 0;
            while (tries < 5)
            {
                try
                {
                    tries++;
                    File.Delete(fileName);
                    Thread.Sleep(200);
                }
                catch { }
            }
        }

        string exceptionMessage;
        public string ExceptionMessage
        {
            get { return exceptionMessage; }
            set { exceptionMessage = value; NotifyPropertyChanged(); }
        }


        public event EventHandler TaskFinished;
        public void OnTaskFinished()
        {
            Progress = 100;
            if (TaskFinished != null)
                TaskFinished(this, EventArgs.Empty);
        }
    }
}
