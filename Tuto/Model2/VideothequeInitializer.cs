using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Navigator.Initialization
{
    public class VideothequeInitializer
    {
        #region Calculating hashes in directory

        public static string CreateHash(FileInfo file)
        {
            using (var md5 = MD5.Create())
            using (var stream = new BinaryReader(File.OpenRead(file.FullName)))
            {
                var bytes = stream.ReadBytes(10000);
                return BitConverter.ToString(md5.ComputeHash(bytes));
            }
        }

        public static void MakeHashes(DirectoryInfo directory, string targetFileName, string hashFileName, bool recomputeAll, Dictionary<DirectoryInfo, string> hashes)
        {
            var files = directory.GetFiles();
            if (files.Any(z => z.Name == targetFileName))
            {
                if (!recomputeAll)
                    if (files.Any(z => z.Name == hashFileName))
                    {
                        hashes[directory] = File.ReadAllText(Path.Combine(directory.FullName, hashFileName));
                        return;
                    }
                var hash = CreateHash(new FileInfo(Path.Combine(directory.FullName, targetFileName)));
                File.WriteAllText(Path.Combine(directory.FullName, hashFileName), hash);
                hashes[directory] = hash;
            }
            else foreach (var d in directory.GetDirectories())
                    MakeHashes(d, targetFileName, hashFileName, recomputeAll, hashes);
        }

        #endregion
    }
}
