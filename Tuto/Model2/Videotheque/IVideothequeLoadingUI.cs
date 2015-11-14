using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model2
{
	public interface IVideothequeLoadingUI
	{
		void StartPOSTWork(string name);
		void CompletePOSTWork(bool result);
		VideothequeLoadingRequestItem Request(string prompt, VideothequeLoadingRequestItem[] items);
		void ExitSuccessfully();
	}
}
