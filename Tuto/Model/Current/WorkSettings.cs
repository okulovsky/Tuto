using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Tuto.BatchWorks;

namespace Tuto.Model
{
    public enum Options {Before, During, Skip};
    public abstract class Settings
    {
        public virtual List<Options> PossibleOptions { get { return new List<Options>() { Options.Before, Options.During, Options.Skip }; } }
        public List<string> OptionsAsStrings { get {return PossibleOptions.Select ( x => x.ToString()).ToList(); }}
        public string CurrentAsString { get; set; }
        public Options CurrentOption { get {return (Options)Enum.Parse(typeof(Options), CurrentAsString);} }
    }

    public class ConversionSettings : Settings
    {
        public override List<Options> PossibleOptions { get { return new List<Options>() { Options.Before, Options.During }; } }
        public ConversionSettings() { CurrentAsString = Options.During.ToString(); }
    }

    public class ThumbSettings : Settings
    {
        public ThumbSettings() { CurrentAsString = Options.Skip.ToString(); }
    }

    public class AudioCleanSettings : Settings
    {
        public AudioCleanSettings() { CurrentAsString = Options.During.ToString(); }
    }

    public class WorkSettings
    {
        public ConversionSettings ConversionSettings { get; set; }
        public ThumbSettings ThumbSettings { get; set; }
        public AudioCleanSettings AudioCleanSettings { get; set; }
        public WorkSettings()
        {
            ConversionSettings = new ConversionSettings();
            ThumbSettings = new ThumbSettings();
            AudioCleanSettings = new AudioCleanSettings();
        }

        public List<BatchWork> GetDuringWorks(EditorModel model)
        {
            var toDo = new List<BatchWork>();
            if (model.Global.WorkSettings.AudioCleanSettings.CurrentOption == Options.During)
                toDo.Add(new CreateCleanSoundWork(model.Locations.FaceVideo, model));

            if (model.Global.WorkSettings.ConversionSettings.CurrentOption == Options.During)
            {
                toDo.Add(new ConvertFaceVideoWork(model));
                toDo.Add(new ConvertDesktopVideoWork(model));
            }

            if (model.Global.WorkSettings.ThumbSettings.CurrentOption == Options.During)
            {
                toDo.Add(new CreateThumbWork(model.Locations.FaceVideo, model));
                toDo.Add(new CreateThumbWork(model.Locations.DesktopVideo, model));
            }
            return toDo;
        }
    }
}
