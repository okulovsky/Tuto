using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Editor
{



    [AttributeUsage(AttributeTargets.All,AllowMultiple = true)]
    public class CmdHelpAttribute : Attribute
    {
        public readonly KeyboardGroup Group;
        public readonly EditorModes? ValidInMode;
        public readonly string HelpMessage;

        public CmdHelpAttribute(EditorModes forMode, string helpMessage)
        {
            this.HelpMessage= helpMessage;
            this.ValidInMode = forMode;
        }

        public CmdHelpAttribute(KeyboardGroup group, string helpMessage)
        {
            this.Group = group;
            this.HelpMessage = helpMessage;
        }
        public CmdHelpAttribute(KeyboardGroup group, EditorModes forMode, string helpMessage) 
        {
            this.Group = group;
            this.HelpMessage = helpMessage; 
            this.ValidInMode = forMode; 
        }
    }
}
