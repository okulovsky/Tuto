using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Navigator
{
    public class TopicsViewModel
    {
        Topic[] root;
        public Topic SelectedItem { get; set; }
        public Topic[] Root { get { return root; } } 
        public RelayCommand Add { get; private set; }
        public RelayCommand Remove { get; private set; }

        public TopicsViewModel(Topic root)
        {
            this.root = new[] { root };
            Add = new RelayCommand(CmAdd);
            Remove = new RelayCommand(CmDelete, () => SelectedItem!=null && SelectedItem.Parent != null);
        }

        void CmAdd()
        { }

        void CmDelete()
        { }

    }
}
