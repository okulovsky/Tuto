using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Tuto.Model;

namespace Tuto.Publishing
{
    [DataContract]
    public class DataLayerRecord<T>
    {
        [DataMember]
        public readonly Guid Guid;
        [DataMember]
        public readonly T Data;

        public DataLayerRecord(Guid guid, T data)
        {
            this.Guid = guid;
            this.Data = data;
        }
    }

    [DataContract]
    public class DataLayer<T>
    {
        [DataMember]
        public readonly List<DataLayerRecord<T>> Records = new List<DataLayerRecord<T>>();
    }
}
