using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Editor
{
    public interface IEditorMode
    {
        void CheckTime();
        void MouseClick(int SelectedLocation, MouseButtonEventArgs button);
        void ProcessKey(KeyboardCommandData key);
    }
}
