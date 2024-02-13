using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class WED : IEAsset
    {
        public WED(string preConversionPath, string postConversionPath, IEResRef resRef) : base(preConversionPath, postConversionPath, resRef)
        {
            ReplaceReference(0x24, "tis", _owningReference.ReferenceBytes);
        }
    }
}
