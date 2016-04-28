using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.ConsoleMode
{
    public class ConsoleLoadingUI : IVideothequeLoadingUI
    {

        public void StartPOSTWork(string name)
        {
            Console.Write(name+"...");
        }

        public void CompletePOSTWork(bool result)
        {
            Console.WriteLine(" OK");
        }

        public VideothequeLoadingRequestItem Request(string prompt, VideothequeLoadingRequestItem[] items)
        {
            Console.WriteLine(prompt);
            Environment.Exit(1);
            return null;
        }

        public void ExitSuccessfully()
        {
            
        }
    }

}
