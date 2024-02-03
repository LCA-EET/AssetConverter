/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class ReferenceGenerator
    {
        private Dictionary<string, string> _referenceTable;
        private Dictionary<int, int> _indexTable;
        private string _filePrefix;
        public  ReferenceGenerator(string filePrefix, string postConversionDirectory)
        {
            _filePrefix = filePrefix;
            _referenceTable = new Dictionary<string, string>();
            _indexTable = new Dictionary<int, int>();
            _indexTable.Add(0, 100000);
            _indexTable.Add(1, 1000);
            _assetTable = new IEAssetTable(postConversionDirectory);
        }

        public bool ReferenceExists(string referenceToCheck)
        {
            //Log.WriteLineToLog(referenceToCheck);
            //Console.ReadLine();
            return _referenceTable.ContainsKey(referenceToCheck.ToLower());
        }
        public byte[] GetReferenceBytes(string oldReferenceID)
        {
            List<byte> toReturn = Encoding.Latin1.GetBytes(GetReference(oldReferenceID).ToUpper()).ToList();
            while(toReturn.Count < 8)
            {
                toReturn.Add(0x00); //null byte
            }
            return toReturn.ToArray();
        }
        public string GetReference(string oldReferenceID)
        {
            return _referenceTable[oldReferenceID.ToLower()];
        }
        public string DetermineReferenceFromBytes(byte[] bytes, int index)
        {
            //references are 8 bytes long
            string toReturn = Encoding.Latin1.GetString(bytes, index, 8);
            int charactersToTrim = 0;
            for(int i = 7; i >= 0; i--)
            {
                if (bytes[index + i] == 0x00)
                {
                    charactersToTrim++;
                }
                else
                {
                    break;
                }
            }
            return toReturn.Substring(0, 8 - charactersToTrim);
        }
        public string ReplaceReference(ref byte[] contents, int offset)
        {
            string reference = DetermineReferenceFromBytes(contents, offset);
            if (ReferenceExists(reference))
            {
                byte[] newReference = GetReferenceBytes(reference);
                for (int j = 0; j < newReference.Length; j++)
                {
                    contents[offset + j] = newReference[j];
                }
                return DetermineReferenceFromBytes(newReference, 0);

            }
            return reference;
        }
        public void AssociateAssetToReference(IEAsset asset)
        {
            if (ReferenceExists(asset.OldReferenceID))
            {
                asset.AssignReferenceID(_referenceTable[asset.OldReferenceID]);
            }
            else
            {
                int nextID = _indexTable[asset.IDIndex];
                _indexTable[asset.IDIndex] = nextID + 1;
                asset.AssignReferenceID(_filePrefix + nextID);
                _referenceTable.Add(asset.OldReferenceID, _filePrefix + nextID);
            }
            _assetTable.AddAsset(asset);
        }
        public void SaveAssetsPostConversion()
        {
            _assetTable.SaveAssetsToPostConversion();
        }
    }
}
*/