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

        public static T ReadWithoutHeader<T>(string text)
        {
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text));
            var data = (T)new DataContractJsonSerializer(typeof(T)).ReadObject(stream);
            return data;
        }

        public static T Read<T>(FileInfo file, string header, int expectedVersion, params Func<FileInfo, string, T>[] updaters)
        {
            var lines = File.ReadAllLines(file.FullName);
            if (!lines[0].StartsWith(header)) throw new Exception("Wrong file format");

            var versionIndex = lines[0].IndexOf(VersionMarker);
            if (versionIndex == -1) throw new Exception("Wrong file format");

            var versionNumberString = lines[0].Substring(versionIndex + VersionMarker.Length, lines[0].Length - versionIndex - VersionMarker.Length);
            int version = 0;
            if (!int.TryParse(versionNumberString, out version)) throw new Exception("Wrong file format");

            var text = lines.Skip(1).Aggregate((a, b) => a + "\n" + b);


            if (version != expectedVersion)
            {
                if (version < 0 || version > expectedVersion) throw new Exception("Wrong version number, expected less or equal to " + expectedVersion);
                if (version >= updaters.Length) throw new Exception("No updater given for version " + version);
                var rightobject=updaters[version](file, text);
                Write<T>(file, header, expectedVersion, rightobject);
                return Read<T>(file, header, expectedVersion, updaters);
            }
            return ReadWithoutHeader<T>(text);
        }

        public static T Read<T>(DirectoryInfo directory)
        {
            var fileName = typeof(T).Name + ".txt";
            var header = typeof(T).Name + " Tuto file";
            var filePath = Path.Combine(directory.FullName, fileName);
            return Read<T>(new FileInfo(filePath), header, 1);
        }



        public static void Write<T>(FileInfo file, string header, int version, T data)
        {
            var stream = new MemoryStream();
            new DataContractJsonSerializer(typeof(T)).WriteObject(stream, data);
            var text = System.Text.Encoding.UTF8.GetString(stream.GetBuffer().Where(z => z != '\0').ToArray()); ;
            using (var writer = new StreamWriter(file.FullName))
            {
                writer.WriteLine(header + VersionMarker + version);
                writer.WriteLine();
                writer.WriteLine(text);
            }
        }

        public static void Write<T>(DirectoryInfo directory, T Data)
        {
            var fileName = typeof(T).Name + ".txt";
            var header = typeof(T).Name + " Tuto file";
            var filePath = Path.Combine(directory.FullName, fileName);
            Write(new FileInfo(filePath), header, 1, Data);
        }
    }
}
