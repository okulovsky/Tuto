using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Tuto.BatchWorks;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Tuto.Navigator
{
    /// <summary>
    /// Interaction logic for BatchWorkWindow.xaml
    /// </summary>
    public partial class BatchWorkWindow : Window
    {
        public BatchWorkWindow()
        {
            InitializeComponent();
        }

        List<BatchWork> work = new List<BatchWork>();
        private Object addLock = new Object();
        bool QueueWorking { get; set; }
        private int currentIndex { get; set; }
        Thread queueThread { get; set; }
        private Process currentProcess;

        private List<Task> tasks;

        void Execute()
        {
            while (currentIndex != this.work.Count && QueueWorking)
            {
                BatchWork e = work[currentIndex];
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
                catch (ThreadAbortException ex)
                {
                    e.Status = BatchWorkStatus.Aborted;
                    e.Clean();
                    currentIndex++;
                }
                catch(Exception ex)
                {
                    e.Status = BatchWorkStatus.Failure;
                    e.ExceptionMessage = ex.Message;
                    e.Clean();
                    currentIndex++;
                };
            }
            QueueWorking = false;
            currentIndex = 0;
            this.work.Clear();
        }

        public void Run(IEnumerable<BatchWork> work)
        {
            work = work.Where(x => !x.Finished() || x.Forced).ToList();
            lock (addLock)
            {
                foreach (var e in work)
                {
                    this.work.AddRange(e.BeforeWorks);
                    this.work.Add(e);
                    this.work.AddRange(e.AfterWorks);
                }     
            }
            if (this.work.Count == 0)
                return;

            this.DataContext = this.work.ToList();
            if (!QueueWorking)
            {
                
                queueThread = new Thread(Execute);
                queueThread.Start();
            }
            QueueWorking = true;
            
            CancelButton.Click += (s, a) =>
            {
                var selectedIndex = this.Tasks.SelectedIndex;
                if (selectedIndex != -1)
                {
                    if (selectedIndex != 0)
                    {
                        this.work[selectedIndex].Status = BatchWorkStatus.Cancelled;
                        return;
                    }
                }


                queueThread.Abort();
                QueueWorking = false;
                if (currentProcess != null && !currentProcess.HasExited)
                {
                    currentProcess.Kill();
                }
                for (var i = currentIndex; i < this.work.Count; i++)
                {
                    if (i == currentIndex)
                    {
                        this.work[i].Status = BatchWorkStatus.Aborted;
                    }
                    else
                        this.work[i].Status = BatchWorkStatus.Cancelled;
                }
                CleanQueue();
            };
            Show();
        }

        private void CleanQueue()
        {
            this.work.Clear();
            currentIndex = 0;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }
    }
}
