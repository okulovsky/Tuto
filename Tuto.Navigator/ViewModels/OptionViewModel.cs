using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public override string ToString()
        {
            return Prompt;
        }
    }

    public class OptionViewModel
    {
        public static IEnumerable<OptionViewModel<T>> FromEnum<T>()
        {
            var type = typeof(T);
            var names = Enum.GetNames(type);
            foreach (var name in names)
            {
                var value = (T)Enum.Parse(type, name);
                var prompt = name;

                try
                {
                    prompt = type.GetMember(name)[0].GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().First().Description;
                }
                catch { }
                yield return new OptionViewModel<T>(value, prompt);
            }
        }
    }
}
