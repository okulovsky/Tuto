using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class PatchSelection
    {
        [DataMember]
        public readonly Patch Item;
        [DataMember]
        public readonly SelectionType Type;
        [DataMember]
        public double SelectionStartX;
        [DataMember]
        public double SelectionStartY;
        public PatchSelection(SelectionType type, Patch item, double selectionStartX, double selectionStartY)
        {
            this.Item = item;
            this.Type = type;
            this.SelectionStartX = selectionStartX;
            this.SelectionStartY = selectionStartY;
        }
    }
}
