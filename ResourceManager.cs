using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
namespace AssetConverter
{
    public static class ResourceManager
    {
        private static Dictionary<string, Dictionary<string, IEResRef>> _assetTable;
        private static Dictionary<string, IEResRef> _resourceQueue;
        private static string _queueFilePath;
        private static string _preConversionDirectory;
        private static string _postConversionDirectory;
        private static string _weiduPath;
        private static string _filePrefix;
        private static int _nextID;
        public static void Initialize(string preConversionDirectory, string postConversionDirectory, string queueFilePath, string weiduPath, string filePrefix)
        {
            _nextID = 1000;
            _filePrefix = filePrefix;
            _resourceQueue = new Dictionary<string, IEResRef>();
            _assetTable = new Dictionary<string, Dictionary<string, IEResRef>>();
            if(!Directory.Exists(preConversionDirectory)) 
            {
                Log.WriteLineToLog("Preconversion directory does not exist. Exiting.");
                Environment.Exit(0);
            }
            else
            {
                _preConversionDirectory = preConversionDirectory;
            }
            if (!Directory.Exists(postConversionDirectory))
            {
                Log.WriteLineToLog("Postconversion directory does not exist. Exiting.");
                Environment.Exit(0);
            }
            else
            {
                _postConversionDirectory = postConversionDirectory;
            }
            if (!File.Exists(queueFilePath))
            {
                Log.WriteLineToLog("Queue file does not exist. Exiting.");
                Environment.Exit(0);
            }
            else
            {
                _queueFilePath = queueFilePath;
            }
            if (!File.Exists(weiduPath))
            {
                Log.WriteLineToLog("Weidu executable does not exist. Exiting.");
                Environment.Exit(0);
            }
            else
            {
                _weiduPath = weiduPath;
            }
            string[] resourcesToLoad = File.ReadAllLines(_queueFilePath);
            foreach (string resource in resourcesToLoad)
            {
                string[] splitResource = resource.ToLower().Split('.');
                AddResourceToQueue(splitResource[0], splitResource[1]);
            }
            LoadResources();
        }
        private static void LoadResources()
        {
            string toLoad;
            List<string> newlyLoaded = new List<string>();
            List<IEResRef> toProcess = new List<IEResRef>();
            foreach(IEResRef resRef in _resourceQueue.Values)
            {
                toProcess.Add(resRef);
            }
            foreach (IEResRef resRef in toProcess)
            {
                Console.WriteLine("Loading reference " + resRef.OldReferenceID + "." + resRef.ResourceType);
                newlyLoaded.Add(resRef.OldReferenceID);
                string assetPath = _preConversionDirectory + resRef.ResourceType + "\\" + resRef.OldReferenceID + "." + resRef.ResourceType;
                string postConversionPath = _postConversionDirectory + resRef.ResourceType + "\\" + resRef.NewReferenceID + "." + resRef.ResourceType;
                IEAsset loadedAsset = null;
                switch (resRef.ResourceType)
                {
                    case "are":
                        Console.WriteLine("... Resource is an area.");
                        loadedAsset = new ARE(assetPath, postConversionPath, resRef);
                        if(!Directory.Exists(_postConversionDirectory + "bmp"))
                        {
                            Directory.CreateDirectory(_postConversionDirectory + "bmp");
                        }
                        List<string> areaBMPs = new List<string>()
                        {
                            _preConversionDirectory + "bmp\\" + resRef.OldReferenceID + "ht.bmp",
                            _preConversionDirectory + "bmp\\" + resRef.OldReferenceID + "lm.bmp",
                            _preConversionDirectory + "bmp\\" + resRef.OldReferenceID + "ln.bmp",
                            _preConversionDirectory + "bmp\\" + resRef.OldReferenceID + "sr.bmp",
                        };
                        foreach(string bmp in areaBMPs)
                        {
                            if (File.Exists(bmp))
                            {
                                File.Copy(bmp, _postConversionDirectory + "bmp\\" + resRef.NewReferenceID + bmp.Substring(bmp.Length - 6, 6), true);
                            }
                        }
                        break;
                    case "baf":
                    case "itm":
                    case "tis":
                    case "wed":
                    case "tra":
                        loadedAsset = new IEAsset(assetPath, postConversionPath, resRef);
                        break;
                    case "dlg":
                        //loadedAsset = new DLG(assetPath, resRef.ResourceType, _weiduPath);
                        break;
                    case "d":
                        //loadedAsset = new D(_preConversionDirectory, resRef.ResourceType);
                        break;
                    case "bmp":
                        //loadedAsset = new BMP(_preConversionDirectory, resRef.ResourceType);
                        break;
                    case "cre":
                        //loadedAsset = new CRE(_preConversionDirectory, resRef.ResourceType);
                        break;
                    case "wav":
                        loadedAsset = new IEAsset(assetPath, postConversionPath, resRef);
                        break;
                }
                if(loadedAsset != null)
                {
                    resRef.SetLoadedAsset(loadedAsset);
                    _assetTable[resRef.ResourceType].Add(resRef.OldReferenceID, resRef);
                }
            }
            foreach (string resourceID in newlyLoaded)
            {
                _resourceQueue.Remove(resourceID);
            }
            if(_resourceQueue.Count > 0)
            {
                LoadResources();
            }
            else
            {
                foreach(string assetType in _assetTable.Keys) 
                {
                    if(!Directory.Exists(_postConversionDirectory + assetType))
                    {
                        Directory.CreateDirectory(_postConversionDirectory + assetType);
                    }
                    Dictionary<string, IEResRef> loaded = _assetTable[assetType];
                    foreach(IEResRef res in loaded.Values)
                    {
                        res.SaveAsset();
                    }
                }
            }
        }
        public static bool IsResourceLoaded(string resourceID, string type)
        {
            if(!_assetTable.ContainsKey(type)) 
            {
                _assetTable.Add(type, new Dictionary<string, IEResRef>());
            }
            return _assetTable[type].ContainsKey(resourceID);
        }
        private static byte[] GetNextResourceID()
        {
            string toReturn = _filePrefix.ToLower() + _nextID;
            _nextID++;
            List<byte> toReturnBytes = Encoding.Latin1.GetBytes(toReturn).ToList();
            while(toReturnBytes.Count < 8)
            {
                toReturnBytes.Add(0x00);
            }
            return toReturnBytes.ToArray();
        }
        public static byte[] GetReplacementResource(string resourceID, string resourceType)
        {
            return _assetTable[resourceType][resourceID].ReferenceBytes;
        }
        public static bool IsInQueue(string resource)
        {
            return _resourceQueue.ContainsKey(resource);
        }

        public static byte[] AddResourceToQueue(string resourceID, string resourceType)
        {
            if (!_resourceQueue.ContainsKey(resourceID))
            {
                if (!_assetTable.ContainsKey(resourceType))
                {
                    _assetTable.Add(resourceType, new Dictionary<string, IEResRef>());
                }
                if (!_assetTable[resourceType].ContainsKey(resourceID))
                {
                    IEResRef toAdd = new IEResRef(resourceID, resourceType, GetNextResourceID());
                    _resourceQueue.Add(resourceID, toAdd);
                    return toAdd.ReferenceBytes;
                }
                else
                {
                    return _assetTable[resourceType][resourceID].ReferenceBytes;
                }
            }
            else
            {
                return _resourceQueue[resourceID].ReferenceBytes;
            }
        }

    }
}
