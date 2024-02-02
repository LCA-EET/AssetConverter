using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class CRE : IEAsset
    {
        private Dictionary<string, string> _stringReferences;
        public CRE(string preConversionPath, string assetType) : base(preConversionPath, assetType)
        {
            _stringReferences = new Dictionary<string, string>();
        }

        public override void SaveAsset(string assetDirectory)
        {
            base.SaveAsset(assetDirectory);
            UpdateStringReferences();
        }
        private void ReplaceReference(ref byte[] inBytes, byte[] replacement)
        {
            for(int i = 0; i < replacement.Length; i++)
            {
                inBytes[i] = replacement[i];
            }
        }
        private uint GetReference(ref byte[] inBytes, int index)
        {
            return BitConverter.ToUInt32(inBytes, index);
        }
        private void UpdateStringReferences()
        {
            byte[] inBytes = File.ReadAllBytes(_postConversionPath);
            _stringReferences.Add("NAME", Program.MasterTRA.GetString(GetReference(ref inBytes, 8)));
            _stringReferences.Add("NAME2", Program.MasterTRA.GetString(GetReference(ref inBytes, 12)));

            for (int i = 164; i < 532; i += 4) // sounds / text
            {
                ReplaceReference(ref inBytes, Program.MasterTRA.AddUsedReference(GetReference(ref inBytes, i)));
            }
            File.WriteAllBytes(_postConversionPath, inBytes);
        }
    }
}
