namespace AssetConverter
{
    internal class IEAssetTable
    {
        private Dictionary<string, Dictionary<string, IEAsset>> _assetTable;
        private string _postConversionDirectory;
        public IEAssetTable(string postConversionDirectory)
        {
            _assetTable = new Dictionary<string, Dictionary<string, IEAsset>>();
            _postConversionDirectory = postConversionDirectory;
        }

        public void AddAsset(IEAsset toAdd)
        {
            if (!_assetTable.ContainsKey(toAdd.AssetType))
            {
                _assetTable.Add(toAdd.AssetType, new Dictionary<string, IEAsset>());
            }
            _assetTable[toAdd.AssetType].Add(toAdd.ReferenceID, toAdd);
        }

        public void SaveAssetsToPostConversion()
        {
            if (!Directory.Exists(_postConversionDirectory))
            {
                Directory.CreateDirectory(_postConversionDirectory);
            }
            foreach(string assetType in _assetTable.Keys)
            {
                Log.WriteLineToLog("Saving " + assetType + " to " + _postConversionDirectory + "...\n");
                string assetDirectory = _postConversionDirectory + assetType + "\\";
                if (!Directory.Exists(assetDirectory))
                {
                    Directory.CreateDirectory(assetDirectory);
                }
                List<IEAsset> assets = _assetTable[assetType].Values.ToList();
                for(int i = 0; i < assets.Count; i++)
                {
                    Log.WriteToLog((i + 1) + " of " + assets.Count);
                    assets[i].SaveAsset(assetDirectory);
                }
                Log.WriteLineToLog("Done.");
            }
        }
    }
}
