using Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Model
{
    public static class EditorModelIO
    {
        public static T ReadJSonWithHeader<T>(FileInfo file, string header)
        {
            var lines = File.ReadAllLines(file.FullName);
            if (!lines[0].StartsWith(header)) throw new Exception("Wrong file format");

            var text = lines.Skip(1).Aggregate((a, b) => a + "\n" + b);
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text));
            var data = (T)new DataContractJsonSerializer(typeof(T)).ReadObject(stream);
            return data;

        }

        public static void WriteJSonWithHeader<T>(FileInfo file, string header, int version, T data)
        {
            var stream = new MemoryStream();
            new DataContractJsonSerializer(typeof(T)).WriteObject(stream, data);
            var text = System.Text.Encoding.UTF8.GetString(stream.GetBuffer().Where(z => z != '\0').ToArray()); ;
            using (var writer = new StreamWriter(file.FullName))
            {
                writer.WriteLine(header + " Version " + version);
                writer.WriteLine();
                writer.WriteLine(text);
            }
        }


        public static string SubstituteDebugDirectories(string subdirectory)
        {
            if (subdirectory.StartsWith("debug\\"))
            {
                subdirectory = subdirectory.Replace("debug\\", "..\\..\\..\\TestModels\\");
            }
            else if (subdirectory.StartsWith("work\\"))
            {
                subdirectory = subdirectory.Replace("work\\", "..\\..\\..\\..\\BP\\");
            }
            return subdirectory;
        }

        static EditorModel InitializeModelsFolder(string subdirectory)
        {
            var localDirectory = new DirectoryInfo(subdirectory);
            if (!localDirectory.Exists) throw new Exception("Local directory '" + subdirectory + "' is not found");
            var rootDirectory = localDirectory;
            while (true)
            {
                try
                {
                    rootDirectory = rootDirectory.Parent;
                    if (rootDirectory == null)
                        throw new Exception();
                }
                catch
                {
                    throw new Exception(string.Format(
                        "Root directory not found. Root directory must be a parent of '{0}' and contain global data file '{1}'",
                        localDirectory.FullName,
                        Locations.GlobalFileName));
                }
                if (rootDirectory.GetFiles(Locations.GlobalFileName).Any())
                    break;
            }

            var programFolder = new FileInfo(Assembly.GetExecutingAssembly().FullName).Directory;

            return new EditorModel(localDirectory, rootDirectory, programFolder);
        }

        static bool TryReadModel(EditorModel model)
        {

            var file = model.VideoFolder.GetFiles(Locations.LocalFileName).FirstOrDefault();
            if (file == null) return false;
            var container = ReadJSonWithHeader<FileContainer>(file, "Tuto local file");
            model.Montage = container.MontageModel;
            model.WindowState = container.WindowState;
            return true;
        }

        public static GlobalData ReadGlobalData(DirectoryInfo rootFolder)
        {
            var file = rootFolder.GetFiles(Locations.GlobalFileName).FirstOrDefault();
            if (file == null)
                return new GlobalData();
            var data=ReadJSonWithHeader<GlobalData>(file, "Tuto project file");
            if (data.TopicsRoot == null) data.TopicsRoot= new Topic();
            data.TopicsRoot.Load();
            return data;
        }

        static bool TryReadObsolete(EditorModel model)
        {
            try {
                return ObsoleteModelIO.LoadAndConvert(model); //try to recover model from obsolete file formats
            }
            catch { }
            return false;
        }

        static void InitializeEmptyModel(EditorModel model)
        {
            model.Montage = new MontageModel(60 * 60 * 1000); //this is very bad. Need to analyze the video file
            model.WindowState = new WindowState();
        }

        public static EditorModel Load(string subdirectory)
        {
            var model = InitializeModelsFolder(subdirectory);
            if (!TryReadModel(model))
            {
                InitializeEmptyModel(model);
                if (!TryReadObsolete(model))
                    InitializeEmptyModel(model);
            }
            model.Global = ReadGlobalData(model.RootFolder);
            return model;
        }

        public static void Save(EditorModel model)
        {
            var container = new FileContainer
            {
                MontageModel = model.Montage,
                WindowState = model.WindowState
            };
            WriteJSonWithHeader<FileContainer>(model.Locations.LocalFilePath, "Tuto local file", 1, container);
        }
    }
}
