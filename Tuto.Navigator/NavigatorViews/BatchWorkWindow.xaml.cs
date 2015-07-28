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

        ConcurrentQueue<BatchWork> work = new ConcurrentQueue<BatchWork>();
        bool QueueWorking { get; set; }
        Thread queueThread { get; set; }

        void Execute()
        {
            while (!work.IsEmpty && QueueWorking)
            {
                BatchWork e;
                work.TryPeek(out e); 
                e.Status = BatchWorkStatus.Running;
                try
                {
                    e.Work();
                    e.Status = BatchWorkStatus.Success;
                    work.TryDequeue(out e);
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
                }
            }
            QueueWorking = false;
        }

        public void Run(IEnumerable<BatchWork> work)
        {
            foreach (var e in work)
                this.work.Enqueue(e);
            this.DataContext = this.work.ToList();
            if (!QueueWorking)
            {
                
                queueThread = new Thread(Execute);
                queueThread.Start();
            }
            QueueWorking = true;
            
            CancelButton.Click += (s, a) =>
            {
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
            BatchWork temp;
            while (!this.work.IsEmpty) this.work.TryDequeue(out temp);
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
