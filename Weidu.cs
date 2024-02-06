using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public static class Weidu
    {
        private static string _weiduPath;
        private static string _weiduDirectory;
        public static void Initialize(string weiduPath)
        {
            _weiduPath = weiduPath;
            _weiduDirectory = Path.GetDirectoryName(_weiduPath) + "\\";
        }

        public static void DecompileDialog(DLG toDecompile)
        {
            Console.WriteLine("Decompiling Dialog " + toDecompile.PreConversionPath);
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo = new ProcessStartInfo(_weiduPath);
            processStartInfo.UseShellExecute = false;
            processStartInfo.WorkingDirectory = Path.GetDirectoryName(toDecompile.PreConversionPath);
            processStartInfo.ArgumentList.Add("--game");
            processStartInfo.ArgumentList.Add(_weiduDirectory);
            processStartInfo.ArgumentList.Add("--trans");
            processStartInfo.ArgumentList.Add(toDecompile.PreConversionPath);
            Process conversion = Process.Start(processStartInfo);
            conversion.WaitForExit();

            string dPath = toDecompile.PreConversionPath.ToLower().Replace(".dlg", ".d");
            string traPath = toDecompile.PreConversionPath.ToLower().Replace(".dlg", ".tra");
            string dDirectory = Directory.GetParent(Path.GetDirectoryName(toDecompile.PostConversionPath)) + "\\d\\";
            string traDirectory = Directory.GetParent(Path.GetDirectoryName(toDecompile.PostConversionPath)) + "\\tra\\";

            if (!Directory.Exists(dDirectory))
            {
                Directory.CreateDirectory(dDirectory);
            }
            if (!Directory.Exists(traDirectory))
            {
                Directory.CreateDirectory(traDirectory);
            }
            string postDPath = dDirectory + toDecompile.OwningReference.NewReferenceID + ".d";
            string postTRAPath = traDirectory + toDecompile.OwningReference.NewReferenceID + ".tra";
            File.Move(dPath, postDPath); 
            File.Move(traPath, postTRAPath);

            toDecompile.SetComponentPaths(postDPath, postTRAPath);
        }

        public static void WeiduGenerateTLK()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(_weiduPath);
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = _weiduDirectory;
            startInfo.ArgumentList.Add("--traify-tlk");
            startInfo.ArgumentList.Add("--out");
            startInfo.ArgumentList.Add("text.tra");
            Process conversion = Process.Start(startInfo);
            conversion.WaitForExit();
            MasterTRA.InitializeMasterTRA(_weiduDirectory + "text.tra");
        }
    }
}
