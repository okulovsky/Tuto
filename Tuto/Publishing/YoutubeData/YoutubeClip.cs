using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
	[DataContract]
    public class YoutubeClip
    {
		[DataMember]
        public string Id { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string Description { get; set; }
		public string VideoURLFull { get { return "http://youtube.com/watch?v=" + Id; } }
		public string GDataURL { get { return "http://gdata.youtube.com/feeds/api/videos/" + Id; } }
        public override string ToString()
        {
            return Name;
        }

		static Regex PrefixRegex = new Regex(@"^[a-zA-Z]+[0-9\-]+ ");

		//TODO: tests!
		public string GetProperName()
		{
			var match = PrefixRegex.Match(Name);

			if (match.Success) return Name.Substring(match.Length, Name.Length - match.Length);
			return Name;
		}

		static Regex GuidRegex = new Regex(@"\[GUID: ([a-f0-9\-]+)\]");

		public static string GuidMarker(Guid guid)
		{
			return "[GUID: " + guid.ToString() + "]";
		}

		public Guid? GetGuid()
		{
			var match = GuidRegex.Match(Description);
			if (match.Success) return Guid.Parse(match.Groups[1].Value);
			return null;
		}

		public void UpdateGuid(Guid? guid)
		{
			string guidString = "";
			if (!guid.HasValue) guidString = ""; 
			else guidString = GuidMarker(guid.Value);
			var match = GuidRegex.Match(Description);
			if (match.Success) Description = GuidRegex.Replace(Description,guidString);
			else Description = Description + "\n" + guidString;
		}
    }
}
