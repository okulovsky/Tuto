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
    public static partial class EditorModelIO
    {


        public static string SubstituteDebugDirectories(string subdirectory)
        {
            if (subdirectory.StartsWith("debug\\"))
            {
                subdirectory = subdirectory.Replace("debug\\", "..\\..\\..\\TestModels\\");
            }
            else if (subdirectory.StartsWith("work\\"))
            {
                subdirectory = subdirectory.Replace("work\\", "D:\\BP\\");
            }
            else if (subdirectory.StartsWith("home\\"))
            {
                subdirectory = subdirectory.Replace("home\\", "..\\..\\..\\..\\Test\\");
            }
            return subdirectory;
        }


    }
}
