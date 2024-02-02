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
        private IEAssetTable _assetTable;
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

        public string GetReference(string oldReferenceID)
        {
            return _referenceTable[oldReferenceID.ToLower()];
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
