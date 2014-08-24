using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Tuto.Navigator;

namespace Tuto.Model
{
    [DataContract]
    public class SubtitleFix : NotifierModel, IComparable, INotifyPropertyChanged
    {
        [DataMember]
        public int startTime;
        public int StartTime { get { return startTime; } set { startTime = value; base.NotifyPropertyChanged(); } }

        [DataMember]
        public int length;
        public int Length { get { return length; } set { length = value; base.NotifyPropertyChanged(); } }

        [DataMember]
        public string text;
        public string Text { get { return text; } set { text = value; base.NotifyPropertyChanged(); } }


        public int CompareTo(object obj)
        {
            return StartTime.CompareTo((obj as SubtitleFix).StartTime);
        }
    }
}
