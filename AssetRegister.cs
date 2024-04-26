using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public static class AssetRegister
    {
        private static Dictionary<string, Dictionary<string, string>> _assetRegister;
        public static void Initialize()
        {
            _assetRegister = new Dictionary<string, Dictionary<string, string>>();
            string[] text = File.ReadAllLines(Program.paramFile.ImportedAssetPath);
            foreach(string line in text)
            {
                string[] split = line.ToLower().Split(",");
                string oldID = split[0];
                string newID = split[1];
                string assetType = split[2];
                if (!_assetRegister.ContainsKey(assetType))
                {
                    _assetRegister.Add(assetType, new Dictionary<string, string>());
                }
                if (!_assetRegister[assetType].ContainsKey(oldID))
                {
                    _assetRegister[assetType].Add(oldID, newID);
                }
            }
        }
        public static bool AlreadyImported(string assetType, string oldID, ref byte[] importedReference) 
        {

            string oldLower = oldID.ToLower();
            if(_assetRegister.ContainsKey(assetType))
            {
                if (_assetRegister[assetType].ContainsKey(oldLower))
                {
                    List<byte> toReturnBytes = Encoding.Latin1.GetBytes(_assetRegister[assetType][oldLower]).ToList();
                    while (toReturnBytes.Count < 8)
                    {
                        toReturnBytes.Add(0x00);
                    }
                    importedReference = toReturnBytes.ToArray();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        public static void AddToRegister(string assetType, string oldID, string newID)
        {
            string oldLower = oldID.ToLower();
            if (!_assetRegister.ContainsKey(assetType))
            {
                _assetRegister.Add(assetType, new Dictionary<string, string>());
            }
            if (!_assetRegister[assetType].ContainsKey(oldLower))
            {
                _assetRegister[assetType].Add(oldLower, newID);
            }
        }
        public static void WriteRegister()
        {
            string output = "";
            foreach(string assetType in _assetRegister.Keys)
            {
                foreach(string oldID in _assetRegister[assetType].Keys)
                {
                    output += oldID + "," + _assetRegister[assetType][oldID] + "," + assetType + Environment.NewLine;
                }
            }
            File.WriteAllText(Program.paramFile.ImportedAssetPath, output); 
        }
    }
}
