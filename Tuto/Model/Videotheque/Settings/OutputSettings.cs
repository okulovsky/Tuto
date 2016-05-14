using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    [DataContract]
    public class OutputSettings
    {
        [DataMember]
        public string SummaryJsonFile { get; set; }

        [DataMember]
        public bool IncludeGUIDInOutputFileName { get; set; }
    }
}
