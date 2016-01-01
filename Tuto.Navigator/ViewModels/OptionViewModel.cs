using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Navigator.ViewModels
{
    public class OptionViewModel<T>
    {
        public T Value { get; private set; }
        public string Prompt { get; private set; }

        public OptionViewModel(T value, string prompt)
        {
            Value = value;
            Prompt = prompt;
        }
    }


}
