using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Editor;

namespace Tuto.Services
{
    enum ExecMode
    {
        Run, Print
    }
    public abstract class Service
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string Help { get; }
        public abstract void DoWork(string[] args);
    }
}
