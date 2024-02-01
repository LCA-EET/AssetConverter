using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class DLG : IEAsset
    {
        public DLG(string preConversionPath, string assetType, string weiduPath) : base(preConversionPath, assetType)
        {
            WeiduGeneration(weiduPath);
        }
        private void WeiduGeneration(string weiduPath)
        {
            string dlgDirectory = Path.GetDirectoryName(_preConversionPath);

            ProcessStartInfo startInfo = new ProcessStartInfo(weiduPath);
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = dlgDirectory;
            startInfo.ArgumentList.Add("--game");
            startInfo.ArgumentList.Add(Path.GetDirectoryName(weiduPath));
            startInfo.ArgumentList.Add("--trans");
            startInfo.ArgumentList.Add(_preConversionPath);
            Process conversion = Process.Start(startInfo);
            conversion.WaitForExit();
            
            string d_generated = _preConversionPath.Replace(".DLG", ".d");
            
            string tra_generated = _preConversionPath.Replace(".DLG", ".tra");
            string d_directory = Path.GetDirectoryName(_preConversionPath) + "\\d\\";
            string tra_directory = Path.GetDirectoryName(_preConversionPath) + "\\tra\\";
            if (!Directory.Exists(d_directory))
            {
                Directory.CreateDirectory(d_directory);
            }
            if (!Directory.Exists(tra_directory))
            {
                Directory.CreateDirectory(tra_directory);
            }
            File.Move(d_generated, d_generated.Replace(dlgDirectory, dlgDirectory + "\\d\\"));
            File.Move(tra_generated, tra_generated.Replace(dlgDirectory, dlgDirectory + "\\tra\\"));
        }
        public override void SaveAsset(string assetDirectory)
        {
           
        }
    }
}
