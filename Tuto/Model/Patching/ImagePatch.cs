using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class ImagePatch :PatchData
    {
        [DataMember]
        public string RelativeFilePath { get; set; }
    }
}
