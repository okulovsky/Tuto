using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{

	public class LoadingUIDecorator : IVideothequeLoadingUI
	{
		IVideothequeLoadingUI innerUI;

		public void StartPOSTWork(string name)
		{
			if (innerUI != null) innerUI.StartPOSTWork(name);
		}

		public void CompletePOSTWork(bool result)
		{
			if (innerUI != null) innerUI.CompletePOSTWork(result);
		}

		public VideothequeLoadingRequestItem Request(string prompt, VideothequeLoadingRequestItem[] items)
		{
			if (innerUI != null)
				return innerUI.Request(prompt, items);
			return null;
		}

		public void ExitSuccessfully()
		{
			if (innerUI != null)
				innerUI.ExitSuccessfully();
		}

		public LoadingUIDecorator(IVideothequeLoadingUI ui)
		{
			innerUI = ui;
		}
	}
}
