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
                    e.Work();
                    e.Status = BatchWorkStatus.Success;
                    currentIndex++;
                }
                catch (ThreadAbortException ex)
                {
                    e.Status = BatchWorkStatus.Aborted;
                    e.Clean();
                    QueueWorking = false;
                }
                catch(Exception ex)
                {
                    e.Status = BatchWorkStatus.Failure;
                    e.ExceptionMessage = ex.Message;
                    e.Clean();
                    QueueWorking = false;
                };
            }
            QueueWorking = false;
            currentIndex = 0;
            this.work.Clear();
        }

        public void Run(IEnumerable<BatchWork> work)
        {
            lock (addLock)
            {
                foreach (var e in work)
                {
                    this.work.AddRange(e.BeforeWorks);
                    this.work.Add(e);
                    this.work.AddRange(e.AfterWorks);
                }     
            }
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
                bool found = false;
                foreach (var e in this.work)
                {
                    if (e.Status == BatchWorkStatus.Aborted) { found = true;}
                    else if (found) e.Status = BatchWorkStatus.Cancelled;
                }
                CleanQueue();
            };
            Show();
        }

        private void CleanQueue()
        {
            this.work.Clear();
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
