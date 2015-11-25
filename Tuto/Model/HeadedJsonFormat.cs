using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    public static class HeadedJsonFormat
    {
        const string VersionMarker= " Version ";

        public static T ReadWithoutHeader<T>(Stream stream)
        {
            var data = (T)new DataContractJsonSerializer(typeof(T)).ReadObject(stream);
			stream.Close();
            return data;
        }

		static void CheckHeader(string actualHeader, string header, int expectedVersion)
		{
			if (header == null) return;
			if (!actualHeader.StartsWith(header)) throw new Exception("Wrong file format");

			var versionIndex = actualHeader.IndexOf(VersionMarker);
			if (versionIndex == -1) throw new Exception("Wrong file format");

			var versionNumberString = actualHeader.Substring(versionIndex + VersionMarker.Length, actualHeader.Length - versionIndex - VersionMarker.Length);
			int version = 0;
			if (!int.TryParse(versionNumberString, out version)) throw new Exception("Wrong file format");
		}
		
		public static Stream OpenAndCheckHeader(FileInfo file, string header, int expectedVersion)
		{
			var stream = File.Open(file.FullName, FileMode.Open, FileAccess.Read);
			List<byte> firstBytes = new List<byte>();
			while(true)
			{
				var bt = stream.ReadByte();
				firstBytes.Add((byte)bt);
				if (bt == '\n') break;
			}
			var firstLine = System.Text.Encoding.UTF8.GetString(firstBytes.ToArray());
			CheckHeader(firstLine, header, expectedVersion);
			return stream;
		}


        public static T Read<T>(FileInfo file, string header, int expectedVersion)
        {
			var stream = OpenAndCheckHeader(file, header, expectedVersion);
			return ReadWithoutHeader<T>(stream);
        }


		public static T Read<T>(FileInfo file)
			where T : new()
		{
			var header = typeof(T).Name + " Tuto file";
			if (!File.Exists(file.FullName)) return new T();
			return Read<T>(file, header, 1);
		}

        public static T Read<T>(DirectoryInfo directory)
            where T : new()
        {
            var fileName = typeof(T).Name + ".txt";
			var filePath = Path.Combine(directory.FullName, fileName);
			return Read<T>(new FileInfo(filePath));
        }



        public static void Write<T>(FileInfo file, string header, int version, T data)
        {
            var stream = new MemoryStream();
            new DataContractJsonSerializer(typeof(T)).WriteObject(stream, data);
            var text = System.Text.Encoding.UTF8.GetString(stream.GetBuffer().Where(z => z != '\0').ToArray()); ;
            dynamic parsedJson = JsonConvert.DeserializeObject(text);
            text = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
            using (var writer = new StreamWriter(file.FullName))
            {
                writer.WriteLine(header + VersionMarker + version);
                writer.WriteLine();
                writer.WriteLine(text);
            }
        }

		public static void Write<T>(FileInfo file, T Data, int version = 1)
		{
			var header = typeof(T).Name + " Tuto file";
			Write(file, header, version, Data);
		}

        public static void Write<T>(DirectoryInfo directory, T Data)
        {
            var fileName = typeof(T).Name + ".txt";
			var filePath = Path.Combine(directory.FullName, fileName);
			Write(new FileInfo(filePath), Data);
        }
    }
}
