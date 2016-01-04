using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    public class EnumNameAttribute : Attribute
    {
        public string DisplayName { get; set; }
        public EnumNameAttribute(string dn) { DisplayName = dn; }
    }
}
