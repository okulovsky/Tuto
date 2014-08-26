using System;
using System.IO;
using System.Text;
using Tuto.Model;

namespace Tuto.TutoServices.Assembler
{
    class AvsContext
    {
        public void AddData(string data)
        {
            internalData.Append(data);
            internalData.AppendLine();
        }

        public string Serialize(EditorModel model)
        {
            return string.Format(Format,
                model.Locations.AvsLibrary.FullName,
                model.Locations.AutoLevelsLibrary.FullName,
                model.Locations.VSFilterLibrary.FullName,
                internalData,
                String.Format(AvsNode.Template, 0));  // root of the tree has id 0
        }

        public int Id { get { id++;
            return id;
        } }

        private int id = -1;
        private const string Format = 
@"import(""{0}"")
loadplugin(""{1}"")
loadplugin(""{2}"")
{3}
return {4}";

        private readonly StringBuilder internalData = new StringBuilder();
        
    }
}