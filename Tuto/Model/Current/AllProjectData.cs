using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    public class AllProjectData
    {
        public GlobalData Global { get; private set; }
        public List<EditorModel> Models { get; private set; }

        public AllProjectData(GlobalData global)
        {
            Global = global;
            Models = new List<EditorModel>();
        }
    }
}
