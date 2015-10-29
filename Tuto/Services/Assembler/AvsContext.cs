using System;
using System.IO;
using System.Text;
using Tuto.Model;

namespace Tuto.TutoServices.Assembler
{
   public class AvsContext
    {
        public void AddData(string data)
        {
            internalData.Append(data);
            internalData.AppendLine();
        }

        public string Serialize(EditorModel model)
        {
            var faceFormat = "";
            var withoutReduction = string.Format(@"face = DirectShowSource(""{0}"").ChangeFPS(25)", model.Locations.ConvertedFaceVideo.FullName) ;
            faceFormat = withoutReduction;
            return string.Format(Format,
                model.Locations.AvsLibrary.FullName,
                model.Locations.AutoLevelsLibrary.FullName,
                model.Locations.VSFilterLibrary.FullName,
                internalData,
                String.Format(AvsNode.Template, 0),
                faceFormat,
                model.Locations.ConvertedDesktopVideo.FullName);  // root of the tree has id 0
        }

       public string GetContent()
        {
            return internalData.ToString();
        }

        public int Id { get { id++;
            return id;
        } }

        private int id = -1;
        private const string Format =
@"import(""{0}"")
{5}
desktop = DirectShowSource(""{6}"").ChangeFPS(25)
{3}
return {4}";

        private readonly StringBuilder internalData = new StringBuilder();
        
    }
}