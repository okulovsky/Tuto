using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto
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

    public class BatchWork : INotifyPropertyChanged
    {
        public string Name { get; set; }
        BatchWorkStatus status;
        public BatchWorkStatus Status { 
            get { return status; } 
            set { status = value; OnPropertyChanged("Status"); }  
        }

        string exceptionMessage;
        public string ExceptionMessage
        {
            get { return exceptionMessage; }
            set { exceptionMessage = value; OnPropertyChanged("ExceptionMessage"); }
        }

        public Action Work { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); 
        }
    }
}
