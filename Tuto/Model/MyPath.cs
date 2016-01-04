using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto
{
    public static class MyPath
    {
        public static string RelativeTo(string path, string root)
        {
            if (!path.StartsWith(root))
                throw new ArgumentException();
            var result = path.Substring(root.Length, path.Length - root.Length);
            if (result.StartsWith("\\"))
                result = result.Substring(1, result.Length - 1);
            return result;
        }

        public static string CreateHierarchicalName(string relativePath)
        {
            return relativePath.Replace("\\", "-");
        }
    }
}
