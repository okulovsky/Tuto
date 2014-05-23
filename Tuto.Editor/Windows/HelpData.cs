using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Editor.Windows
{
    public class ModeHelp
    {
        public EditorModes Mode { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public List<GroupHelp> Groups { get; set; }
    }

    public class GroupHelp
    {
        public ModeHelp ModeHelp { get; set; }
        public KeyboardGroup Group { get; set; }
        public string Name { get; set; }
        public string CommonText { get; set; }
        public string ModeText { get; set; }
        public List<CommandHelp> Commands { get; set; }
    }

    public class CommandHelp
    {
        public KeyboardCommands Command { get; set; }
        public string KeySymbol { get; set; }
        public string Text { get; set; }
    }

    public static class HelpCreator
    {
        public static IEnumerable<Tuple<T, Attribute[]>> GetEnumAttributes<T>()
        {
            return Enum
                .GetValues(typeof(T))
                .Cast<T>()
                .Select(z => Tuple.Create(z, typeof(T).GetMember(z.ToString())[0].GetCustomAttributes(false).Cast<Attribute>().ToArray()));

        }

        public static string FirstOr<TIn>(this IEnumerable<TIn> e, Func<TIn,string> selector)
        {
            var data = e.FirstOrDefault();
            if (data == null) return "";
            return selector(data);
        }

        public static TOut FirstOr<TIn,TOut>(this IEnumerable<TIn> e, Func<TIn, TOut> selector, TOut defaultValue)
        {
            var data = e.FirstOrDefault();
            if (data == null) return defaultValue;
            return selector(data);
        }

        public static List<ModeHelp> CreateModeHelp()
        {
            var modes =  GetEnumAttributes<EditorModes>()
                .Select(z =>
                    new ModeHelp
                    {
                        Mode = z.Item1,
                        Name = z.Item2.OfType<DisplayNameAttribute>().FirstOr(x=>x.DisplayName),
                        Text = z.Item2.OfType<DescriptionAttribute>().FirstOr(x=>x.Description)
                    })
                .ToList();
            return modes;
        }

        public static List<GroupHelp> CreateGroupHelp(ModeHelp help)
        {
            var groups = GetEnumAttributes<KeyboardGroup>()
                .Select(z =>
                    new GroupHelp
                    {
                        ModeHelp = help,
                        Group = z.Item1,
                        Name = z.Item2.OfType<DisplayNameAttribute>().FirstOr(x => x.DisplayName),
                        CommonText = z.Item2.OfType<DescriptionAttribute>().FirstOr(x => x.Description),
                        ModeText = z.Item2.OfType<CmdHelpAttribute>().Where(x => x.ValidInMode == help.Mode).FirstOr(x => x.HelpMessage),
                    })
                .ToList();
            return groups;
        }

        public static List<CommandHelp> CreateCommandHelp(GroupHelp help)
        {
            return null;

        }


    }


}
