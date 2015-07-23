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

namespace Tuto.BatchWorks
{
    public enum BatchWorkStatus
    {
        Pending,
        Running,
        Success,
        Failure,
        Aborted,
        Cancelled
    }

    public abstract class BatchWork : INotifyPropertyChanged
    {
        public virtual Process Process { get; set; }
        public virtual EditorModel Model { get; set; }
        public string FullPath { get; set; }
        public string Args { get; set; }
        public void RunProcess()
        {
            Process = new Process();
            Process.StartInfo.FileName = FullPath;
            Process.StartInfo.Arguments = Args;
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.CreateNoWindow = true;
            Process.Start();
            Process.WaitForExit();
            try {
                if (Process.ExitCode != 0)
                    throw new ArgumentException("Failed");
            }
            catch { throw new ArgumentException("Failed"); }
        }

        public virtual void Work() {}
        public virtual void Clean() {}

        public string Name { get; set; }
        BatchWorkStatus status;
        public BatchWorkStatus Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged("Status"); }
        }


        string exceptionMessage;
        public string ExceptionMessage
        {
            get { return exceptionMessage; }
            set { exceptionMessage = value; OnPropertyChanged("ExceptionMessage"); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
