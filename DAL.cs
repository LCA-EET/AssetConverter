using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public static class DAL
    {
        private static HashSet<string> _destinationAssets;
        public static void Initialize()
        {
            _destinationAssets = new HashSet<string>();
            string[] lines = File.ReadAllLines(Program.paramFile.DestinationAssetList);
            foreach(string line in lines)
            {
                _destinationAssets.Add(line.ToLower().Trim());
            }
        }
        public static bool AssetExistsInDestination(string assetName, string assetType)
        {
            string toCheck = (assetName + "." + assetType).ToLower();
            return _destinationAssets.Contains(toCheck);
        }
    }
}
