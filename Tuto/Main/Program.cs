using Editor;
using Tuto.TutoServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Tuto.Model;

namespace Tuto
{
    public class TutoProgram
    {
        public static IEnumerable<BatchWork> MakeAll(string fullPathOfModel, bool forceMontage=false)
        {
            var dir=new DirectoryInfo(fullPathOfModel);
            var model = EditorModelIO.Load(fullPathOfModel);
            if (forceMontage || !model.Montage.Montaged)
                yield return new BatchWork 
                    { 
                        Name = "Montaging " + dir.Name,
                        Work = () => Run(Services.Montager, dir)
                    };

            yield return new BatchWork 
                    { 
                        Name = "Assembling " + dir.Name,
                        Work = ()=> Run(Services.Assembler, dir)
                    };
        }

        public static void Run(Services service, DirectoryInfo directory, ExecMode mode = ExecMode.Run)
        {
            Main(new string[] { service.ToString(), directory.FullName, mode.ToString() });
        }

        public static void Repair(DirectoryInfo directory, bool face, ExecMode mode = ExecMode.Run)
        {
            Main(Services.Repair.ToString(), directory.FullName, face ? "face" : "desktop", mode.ToString());
        }

        public static void SetSilentMode()
        {
            Shell.SilentMode = true;
        }

        public static void Main(params string[] args)
        {
            var services = new List<Service>
            {
                new PraatService(),
                new MontagerService(),
                new AssemblerService(),
                new RepairService(),
            };

            if (args.Length < 1)
            {
                var serviceDescriptions = String.Join("\n", services.Select(s => String.Format("{0}\t{1}", s.Name, s.Description)));
                Console.WriteLine(Help, serviceDescriptions);
                return;
            }
            
            AppDomain.CurrentDomain.UnhandledException += (sender, a) =>
                {
                    Console.Error.WriteLine((a.ExceptionObject as Exception).Message);
                 //   Environment.Exit(1); //TODO: раскомментить эту строчку для релиза
                };

            //args[1] = ModelIO.DebugSubdir(args[1]);

            var service = services.Find(s => s.Name.ToLower() == args[0].ToLower());
            if(service == null)
                throw new Exception("Service " + args[0] + " is not recognized");
            if (args.Length == 1)
            {
                Console.WriteLine(
                    "{0} service usage: {1}",
                    service.Name,
                    service.Help
                    );
                return;
            }
            service.DoWork(args);
        }



        const string Help = @"
Usage: NewName <service> [args]

service: one of the available services:
{0}

Run service without args to get specific help.
";


    }
}
