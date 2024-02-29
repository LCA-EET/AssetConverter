using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class TIS : IEAsset
    {
        private Dictionary<string, string> _pvrzTable;
        public TIS(string preConversionPath, string postConversionPath, IEResRef resRef) : base(preConversionPath, postConversionPath, resRef)
        {
            _pvrzTable = new Dictionary<string, string>();
            string tisDirectory = Path.GetDirectoryName(preConversionPath);

            string pvrPrefix = resRef.OldReferenceID.Substring(0, 1);
            pvrPrefix += resRef.OldReferenceID.Substring(2);
            string newPrefix = resRef.NewReferenceID.Substring(0, 1);
            newPrefix += resRef.NewReferenceID.Substring(2);
            string[] files = Directory.GetFiles(tisDirectory);
            foreach(string file in files)
            {
                string fileLower = file.ToLower();
                if (fileLower.EndsWith(".pvrz"))
                {
                    fileLower = Path.GetFileName(fileLower);
                    if (fileLower.StartsWith(pvrPrefix))
                    {
                        string newPVRZName = Path.GetFileName(fileLower).Replace(pvrPrefix, newPrefix);
                        ResourceManager.AddPVRZ(tisDirectory + "\\" + fileLower, Program.paramFile.PostconversionDirectory + "pvrz\\" + newPVRZName);
                    }
                }
            }
        }
    }
}
