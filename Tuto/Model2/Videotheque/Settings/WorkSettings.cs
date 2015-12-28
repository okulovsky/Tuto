using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Tuto.BatchWorks;

namespace Tuto.Model
{
    //NEED REFACTORING!

    public enum Options {BeforeEditing, DuringEditing, Skip, WithAssembly};
    public abstract class Settings
    {
        public virtual List<Options> PossibleOptions { get { return new List<Options>() { Options.BeforeEditing, Options.DuringEditing, Options.Skip, Options.WithAssembly }; } }
        public List<string> OptionsAsStrings { get {return PossibleOptions.Select ( x => x.ToString()).ToList(); }}
        public string CurrentAsString { get; set; }
        public Options CurrentOption { get {return (Options)Enum.Parse(typeof(Options), CurrentAsString);} }
    }

    public class ConversionSettings : Settings
    {
        public override List<Options> PossibleOptions { get { return new List<Options>() { Options.BeforeEditing, Options.DuringEditing, Options.WithAssembly }; } }
        public ConversionSettings() { CurrentAsString = Options.DuringEditing.ToString(); }
    }

    public class ThumbSettings : Settings
    {
        public ThumbSettings() { CurrentAsString = Options.Skip.ToString(); }
    }

    public class DesktopThumbSettings : Settings
    {
        public DesktopThumbSettings() { CurrentAsString = Options.Skip.ToString(); }
        public override List<Options> PossibleOptions { get { return new List<Options>() { Options.BeforeEditing, Options.DuringEditing, Options.Skip }; } }
    }

    public class FaceThumbSettings : Settings
    {
        public FaceThumbSettings() { CurrentAsString = Options.Skip.ToString(); }
        public override List<Options> PossibleOptions { get { return new List<Options>() { Options.BeforeEditing, Options.DuringEditing, Options.Skip }; } }
    }

    public class AudioCleanSettings : Settings
    {
        public AudioCleanSettings() { CurrentAsString = Options.WithAssembly.ToString(); }
        public override List<Options> PossibleOptions { get { return new List<Options>() { Options.Skip, Options.WithAssembly, Options.DuringEditing }; } }
    }

    [DataContract]
    public class WorkSettings
    {
        [DataMember]
        public ConversionSettings ConversionSettings { get; set; }
        [DataMember]
        public AudioCleanSettings AudioCleanSettings { get; set; }
        [DataMember]
        public FaceThumbSettings FaceThumbSettings { get; set; }
        [DataMember]
        public DesktopThumbSettings DesktopThumbSettings { get; set; }
        [DataMember]
        public bool AutoUploadVideo { get; set; }

        [DataMember]
        public bool ShowProcesses { get; set; }


        [DataMember]
        public bool StartPraat { get; set; }

        public WorkSettings()
        {
            ConversionSettings = new ConversionSettings();
            AudioCleanSettings = new AudioCleanSettings();
            FaceThumbSettings = new FaceThumbSettings();
            DesktopThumbSettings = new DesktopThumbSettings();
            StartPraat = true;
        }

        public List<BatchWork> GetDuringWorks(EditorModel model)
        {
            var toDo = new List<BatchWork>();
            if (model.Videotheque.Data.WorkSettings.AudioCleanSettings.CurrentOption == Options.DuringEditing)
                toDo.Add(new CreateCleanSoundWork(model.Locations.FaceVideo, model, false));

            if (model.Videotheque.Data.WorkSettings.ConversionSettings.CurrentOption == Options.DuringEditing)
            {
                toDo.Add(new ConvertFaceWork(model, false));
                toDo.Add(new ConvertDesktopWork(model, false));
            }

            if (model.Videotheque.Data.WorkSettings.FaceThumbSettings.CurrentOption == Options.DuringEditing)
            {
                toDo.Add(new CreateThumbWork(model.Locations.FaceVideo, model, false));
            }

            if (model.Videotheque.Data.WorkSettings.DesktopThumbSettings.CurrentOption == Options.DuringEditing)
            {
                toDo.Add(new CreateThumbWork(model.Locations.DesktopVideo, model, false));
            }            
            return toDo;
        }

        public List<BatchWork> GetBeforeEditingWorks(EditorModel model)
        {
            var works = new List<BatchWork>();
            if (model.Videotheque.Data.WorkSettings.FaceThumbSettings.CurrentOption == Options.BeforeEditing)
                works.Add(new CreateThumbWork(model.Locations.FaceVideo, model, false));

            if (model.Videotheque.Data.WorkSettings.DesktopThumbSettings.CurrentOption == Options.BeforeEditing)
                works.Add(new CreateThumbWork(model.Locations.DesktopVideo, model, false));

            if (model.Videotheque.Data.WorkSettings.AudioCleanSettings.CurrentOption == Options.BeforeEditing)
                works.Add(new CreateCleanSoundWork(model.Locations.FaceVideo, model, false));

            if (model.Videotheque.Data.WorkSettings.ConversionSettings.CurrentOption == Options.BeforeEditing)
            {
                works.Add(new ConvertDesktopWork(model, false));
                works.Add(new ConvertFaceWork(model, false));
            }
            return works;
        }
    }
}
