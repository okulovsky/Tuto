using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Navigator
{
    public class TopicViewModel
    {
        public Guid Guid { get; set; }
        public string Caption { get; set; }
        public TopicViewModel Parent { get; set; }
        public bool Selected { get; set; }
        public ObservableCollection<object> Items { get; set; }

        public TopicViewModel()
        {
            Items = new ObservableCollection<object>();
        }
    }
}
