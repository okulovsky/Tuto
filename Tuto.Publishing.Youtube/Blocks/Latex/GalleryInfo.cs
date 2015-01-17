using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
	[DataContract]
	class GalleryInfo
	{
        [DataMember]
        public DateTime CompilationTime { get; set; }
		[DataMember]
		public DirectoryInfo Directory { get; set; }
	}
}
