using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class IEResRef
    {
        private string _oldReferenceID;
        private string _newReferenceID;
        private string _resourceType;
        private byte[] _newReferenceBytes;
        private IEAsset _loadedAsset;

        public IEResRef(string resource, string resourceType, byte[] resourceID)
        {
            _oldReferenceID = resource;
            _resourceType = resourceType;
            _newReferenceBytes = resourceID;
            int charsToTrim = 0;
            for (int j = 7; j >= 0; j--) 
            {
                if (resourceID[j] != 0x00)
                {
                    break;
                }
                else
                {
                    charsToTrim++;
                }
            }
            _newReferenceID = Encoding.Latin1.GetString(resourceID).Substring(0, 8 - charsToTrim);
        }
        public byte[] ReferenceBytes
        {
            get
            {
                return _newReferenceBytes;
            }
        }
        public string NewReferenceID
        {
            get
            {
                return _newReferenceID;
            }
        }
        public string OldReferenceID
        {
            get
            {
                return _oldReferenceID;
            }
        }

        public string ResourceType
        {
            get
            {
                return _resourceType;
            }
        }

        public void SetLoadedAsset(IEAsset loaded)
        {
            _loadedAsset = loaded;
        }

        public void SaveAsset()
        {
            _loadedAsset.SaveAsset();
        }
    }
}
