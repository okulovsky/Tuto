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

        IEnumerable<BatchWork> work;

        void Execute()
        {
            foreach (var e in work)
            {
                e.Status = BatchWorkStatus.Running;
                try
                {
                    e.Work();
                    e.Status = BatchWorkStatus.Success;
                }
                catch (ThreadAbortException ex)
                {
                    e.Status = BatchWorkStatus.Aborted;
                }
                catch(Exception ex)
                {
                    e.Status = BatchWorkStatus.Failure;
                    e.ExceptionMessage = ex.Message;
                }
            }
        }

        public void Run(IEnumerable<BatchWork> work)
        {
            this.work=work;
            this.DataContext = work;
            var thread = new Thread(Execute);
            thread.Start();
            CancelButton.Click += (s, a) =>
                {
                    thread.Abort();
                    bool found = false;
                    foreach (var e in work)
                    {
                        if (e.Status == BatchWorkStatus.Aborted) found = true;
                        else if (found) e.Status = BatchWorkStatus.Cancelled;
                    }
                };
            Show();
        }
    }
}
