using System;
using System.IO;
using System.Text;

namespace Tuto.Services.Assembler
{
    class AvsContext
    {
        public void AddData(string data)
        {
            internalData.Append(data);
            internalData.AppendLine();
        }

        public string Serialize(FileInfo avsLibrary, FileInfo autolevelsLibrary)
        {
            return string.Format(Format,
                avsLibrary.FullName,
                autolevelsLibrary.FullName,
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
{2}
return {3}";

        private readonly StringBuilder internalData = new StringBuilder();
        
    }
}