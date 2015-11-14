using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Editor;
using System.Windows;

namespace Tuto.Model
{
    public static partial class EditorModelIO
    {
        const string localFileHeader = "Tuto local file";
        const int CurrentLocalVersion = 3;

        public static void Save(EditorModel model)
        {
            var container = new FileContainer
            {
                MontageModel = model.Montage,
                WindowState = model.WindowState
            };
            HeadedJsonFormat.Write<FileContainer>(model.Locations.LocalFilePath, localFileHeader, CurrentLocalVersion, container);
        }

        static bool TryReadModel(EditorModel model)
        {

            var file = model.VideoFolder.GetFiles(Locations.LocalFileName).FirstOrDefault();
            if (file == null) return false;
            var container = HeadedJsonFormat.Read<FileContainer>(file, localFileHeader, CurrentLocalVersion, null, UpdateLocalV1, UpdateLocalV2, UpdateLocalV3);

            model.Montage = container.MontageModel;
            model.WindowState = container.WindowState;


            return true;
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

            var programFolder = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
			return new EditorModel(localDirectory, rootDirectory, programFolder);
        }

        static bool TryReadObsolete(EditorModel model)
        {
            
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
            model.Videotheque = ReadGlobalData(model.RootFolder);
            if (!model.Locations.TemporalDirectory.Exists)
                model.Locations.TemporalDirectory.Create();
            return model;
        }


    }
}
