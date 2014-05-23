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
            T data = (T)new DataContractJsonSerializer(typeof(T)).ReadObject(stream);
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
                subdirectory = subdirectory.Replace("work\\", "..\\..\\..\\..\\AIML-VIDEO\\");
            }
            return subdirectory;
        }

        static EditorModel InitializeModelsFolder(string subdirectory)
        {
            var localDirectory = new DirectoryInfo(subdirectory);
            var path = localDirectory.FullName;
            if (!localDirectory.Exists) throw new Exception("Local directory '" + subdirectory + "' is not found");
            var rootDirectory = localDirectory;
            while (true)
            {
                try
                {
                    rootDirectory = rootDirectory.Parent;
                }
                catch
                {
                    throw new Exception("Root directory is not found. Root directory must be a parent of '" + localDirectory.FullName + "' and contain global data file '" + Locations.GlobalFileName + "'");
                }
                if (rootDirectory.GetFiles(Locations.GlobalFileName).Length != 0)
                    break;
            }

            var programFolder = new FileInfo(Assembly.GetExecutingAssembly().FullName).Directory;

            EditorModel model = new EditorModel(localDirectory, rootDirectory, programFolder);
            return model;
        }

        static bool TryReadModel(EditorModel model)
        {

            var file = model.VideoFolder.GetFiles(Locations.LocalFileName).FirstOrDefault();
            if (file == null) return false;
            FileContainer container = ReadJSonWithHeader<FileContainer>(file, "Tuto local file");
            model.Montage = container.MontageModel;
            model.WindowState = container.WindowState;
            return true;
        }

        static void ReadGlobalData(EditorModel model)
        {

            var file = model.RootFolder.GetFiles(Locations.GlobalFileName).FirstOrDefault();
            if (file == null)
            {
                model.Global = new GlobalData();
            }
            else
            {
                model.Global = ReadJSonWithHeader<GlobalData>(file, "Tuto project file");
            }
        }

        static bool TryReadObsolete(EditorModel model)
        {
            //try {
                return ObsoleteModelIO.LoadAndConvert(model); //try to recover model from obsolete file formats
            /*}
            catch { }
            return false;
        */
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
            ReadGlobalData(model);
            return model;
        }

        public static void Save(EditorModel model)
        {
            model.CreateFileChunks();
            var container = new FileContainer();
            container.MontageModel = model.Montage;
            container.WindowState = model.WindowState;
            WriteJSonWithHeader<FileContainer>(model.Locations.LocalFilePath, "Tuto local file", 1, container);
        }
    }
}
