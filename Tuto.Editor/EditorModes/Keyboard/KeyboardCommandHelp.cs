using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Editor
{

    public class OfGroupAttribute : Attribute
    {
        public KeyboardGroup Group { get; set; }
        public OfGroupAttribute(KeyboardGroup group)
        {
            Group = group;
        }
    }

    [AttributeUsage(AttributeTargets.All,AllowMultiple = true)]
    public class CmdHelpAttribute : Attribute
    {
        public readonly EditorModes? ValidInMode;
        public readonly string HelpMessage;

        public CmdHelpAttribute(EditorModes forMode, string helpMessage)
        {
            this.HelpMessage= helpMessage;
            this.ValidInMode = forMode;
        }

        public CmdHelpAttribute(string helpMessage)
        {
            this.HelpMessage = helpMessage;
        }
      
    }
}
