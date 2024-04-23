using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class BAM : IEAsset
    {
        public BAM(string preConversionPath, string postConversionPath, IEResRef resRef) : base(preConversionPath, postConversionPath, resRef)
        {
            //_text = File.ReadAllText(preConversionPath);
            if (_contents[5] == 0x32) // BAM V2 uses PVRZ
            {
                //read the PVRZ index
                int pvrz = BitConverter.ToInt32(_contents, 0x30);
                string oldPath = Program.paramFile.PreconversionDirectory + "\\pvrz\\mos" + pvrz + ".pvrz";
                string newPath = Program.paramFile.PostconversionDirectory + "\\pvrz\\mos" + pvrz + ".pvrz";
                ResourceManager.AddPVRZ(oldPath.ToLower(), newPath.ToLower());
            }
        }
    }
}
