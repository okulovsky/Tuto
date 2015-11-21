using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tuto.Model;

namespace Tuto.Navigator.NewLook
{
  public  class EpisodeViewModel
    {
        EpisodInfo info;

        public Visibility LinkIsVisible { get { return info.YoutubeId == null ? Visibility.Collapsed : Visibility.Visible; } }
       
        public string Name { get { return info.Name; } }


        public RelayCommand Open { get; private set;}

        public EpisodeViewModel(EpisodInfo info)
      {
          this.info=info;
          Open = new RelayCommand(()=>Process.Start("http://youtube.com/watch?v="+info.YoutubeId));
      }
    }
}
