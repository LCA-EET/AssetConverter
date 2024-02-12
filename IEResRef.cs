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
        private IEResRef(string resource, string resourceType)
        {
            _oldReferenceID = resource;
            _resourceType = resourceType;
        }
        public IEResRef(string resource, string resourceType, string newReferenceID) : this(resource, resourceType)
        {
            _newReferenceID = newReferenceID;
        }
        public IEResRef(string resource, string resourceType, byte[] resourceID) : this(resource, resourceType)
        {
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


        public void SaveAsset()
        {
            _loadedAsset.SaveAsset();
        }

        public IEAsset LoadedAsset{
            get
            {
                return _loadedAsset;
            }
            set
            {
                _loadedAsset = value;
            }
        }
    }
}
