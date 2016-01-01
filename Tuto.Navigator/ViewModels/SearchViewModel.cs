using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Navigator.ViewModels
{
    public class SearchViewModel : NotifierModel
    {
        string textSearch;
        public string TextSearch
        {
            get { return textSearch; }
            set
            {
                textSearch = value;
                NotifyPropertyChanged();
            }
        }

        bool onlyWithSource;
        public bool OnlyWithSource
        {
            get { return onlyWithSource; }
            set
            {
                onlyWithSource = value;
                NotifyPropertyChanged();
            }
        }
    }
}
