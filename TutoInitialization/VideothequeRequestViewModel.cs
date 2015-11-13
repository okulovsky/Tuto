using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Init
{
	public class VideothequeRequestViewModel : NotifierModel
	{
		public string Prompt { get; set; }
		public VideothequeLoadingRequestItem[] Items { get; set; }
		public bool OkIsEnabled { get { return Items.Length > 0 && SelectedItem!=null; } }

		VideothequeLoadingRequestItem selectedItem;

		public VideothequeLoadingRequestItem SelectedItem
		{
			get { return selectedItem; }
			set
			{
				selectedItem = value;
				base.NotifyPropertyChanged();
				this.NotifyByExpression(z => z.OkIsEnabled);
			}
		}
		
	}
}
