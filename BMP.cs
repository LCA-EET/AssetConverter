using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class BMP : IEAsset
    {
        private string _suffix;
        public BMP(string preConversionPath, string assetType) : base(preConversionPath, assetType)
        {
            _suffix = _oldName.Substring(_oldName.Length - 2, 2).ToLower();
        }

        public bool IsAreaImage
        {
            get
            {
                switch (_suffix)
                {
                    case "ht":
                    case "lm":
                    case "ln":
                    case "sr":
                        return true;
                }
                return false;
            }
            
        }
        
        public override void AssignReferenceID(string referenceID)
        {
            base.AssignReferenceID(IsAreaImage ? (referenceID + _suffix) : referenceID);
        }
    }
}
