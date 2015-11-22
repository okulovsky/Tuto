using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Tuto.Model;

namespace Tuto.Init
{
    public class VideothequeItemViewModel : NotifierModel
    {
        public VideothequeLoadingRequestItem Item { get; set; }
        bool selected;
        public bool Selected
        {
            get { return selected; }
            set { selected = value; base.NotifyPropertyChanged(); }
        }
    }


	public class VideothequeRequestViewModel : NotifierModel
	{
		public string Prompt { get; private set; }
        public VideothequeItemViewModel[] Items { get; private set; }
        public bool OkIsEnabled { get { return Items.Length > 0 && Items.Any(z => z.Selected); } }

        public bool Cancelled { get; set; }

        public VideothequeRequestViewModel(string prompt, VideothequeLoadingRequestItem[] items)
        {
            Prompt = prompt;
            Items = items.Select(z => new VideothequeItemViewModel { Item = z, Selected = false }).ToArray();
            foreach(var e in Items)
            {
                e.PropertyChanged += (sender, args) => { if (args.PropertyName == "Selected") this.NotifyByExpression(z => z.OkIsEnabled); }; 
            }
        }
	}
}
