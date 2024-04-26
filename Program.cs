using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Text;
namespace AssetConverter
{
    internal class Program
    {

        public static string ParamPath;
        public static ParamFile paramFile;
        static void Main(string[] args)
        {
            
            string paramsFile = "params.in";
            if(args.Length > 0 )
            {
                if (File.Exists(args[0]))
                {
                    paramsFile = args[0];
                }
                else
                {
                    if (!File.Exists(paramsFile))
                    {
                        Console.WriteLine("Params file not found.");
                        Environment.Exit(0);
                    }
                }
            }
            if (!File.Exists(paramsFile))
            {
                Console.WriteLine("Params file not found.");
                Environment.Exit(0);
            }
            paramFile = new ParamFile(paramsFile);
            ParamPath = paramsFile;            
            if(Directory.Exists(paramFile.PostconversionDirectory))
            {
                Directory.Delete(paramFile.PostconversionDirectory, true);
            }
            Directory.CreateDirectory(paramFile.PostconversionDirectory);
            Weidu.Initialize(paramFile.WeiduPath);
            DAL.Initialize();
            ResourceManager.Initialize(paramFile.PreconversionDirectory,
                paramFile.PostconversionDirectory,
                paramFile.QueuePath,
                paramFile.WeiduPath,
                paramFile.Prefix,
                paramFile.ModFolder
                );
            MusicTable.Initialize(paramFile.MusicIndex, paramFile.SongListPath, paramFile.MusicDirectory);
            Weidu.WeiduGenerateTLK();
            AssetRegister.Initialize();
            ResourceManager.ProcessResources();
        }    
        
    }
}
