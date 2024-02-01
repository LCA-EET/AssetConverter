using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class WAV : IEAsset
    {
        public WAV(string preConversionPath, string assetType) : base(preConversionPath, assetType)
        {

        }
        public bool IsAmbient
        {
            get
            {
                return _oldName.StartsWith("am");
            }
            
        }
    }
}
