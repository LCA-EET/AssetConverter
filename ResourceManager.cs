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
        private static Dictionary<string, string> _dialogRegistry;
        private static string _queueFilePath;
        private static string _preConversionDirectory;
        private static string _postConversionDirectory;
        private static string _weiduPath;
        private static string _filePrefix;
        private static string _modFolder;
        private static HashSet<string> _doNotLoad;
        private static int _nextID;
        public static void Initialize(string preConversionDirectory, string postConversionDirectory, string queueFilePath, string weiduPath, string filePrefix, string modFolder)
        {
            _nextID = 1000;
            _filePrefix = filePrefix;
            _doNotLoad  = new HashSet<string>();
            _resourceQueue = new Dictionary<string, IEResRef>();
            _dialogRegistry = new Dictionary<string, string>();
            _assetTable = new Dictionary<string, Dictionary<string, IEResRef>>();
            _modFolder = modFolder;
            if(!Directory.Exists(preConversionDirectory)) 
            {
                Console.WriteLine("Preconversion directory does not exist. Exiting.");
                Environment.Exit(0);
            }
            else
            {
                _preConversionDirectory = preConversionDirectory;
            }
            if (!Directory.Exists(postConversionDirectory))
            {
                Console.WriteLine("Postconversion directory does not exist. Exiting.");
                Environment.Exit(0);
            }
            else
            {
                _postConversionDirectory = postConversionDirectory;
            }
            if (!File.Exists(queueFilePath))
            {
                Console.WriteLine("Queue file does not exist. Exiting.");
                Environment.Exit(0);
            }
            else
            {
                _queueFilePath = queueFilePath;
            }
            if (!File.Exists(weiduPath))
            {
                Console.WriteLine("Weidu executable does not exist. Exiting.");
                Environment.Exit(0);
            }
            else
            {
                _weiduPath = weiduPath;
            }
            
        }
        public static string ReadString_Latin1(byte[] contents, int start, int length)
        {
            byte[] toProcess = new byte[length];
            int index = 0;
            for(int i = start; i < (start + length); i++)
            {
                toProcess[index] = contents[i];
                index++;
            }
            return Encoding.Latin1.GetString(TrimTrailingNullBytes(toProcess));
        }
        public static byte[] TrimTrailingNullBytes(byte[] bytes)
        {
            List<byte> toReturn = new List<byte>();
            toReturn.AddRange(bytes);
            int toTrim = 0;
            for(int j = bytes.Length-1; j >= 0; j--)
            {
                if(bytes[j] == 0x00)
                {
                    toTrim++;
                }
            }
            return toReturn.GetRange(0, toReturn.Count - toTrim).ToArray();
        }
        public static void ProcessResources()
        {
            string[] resourcesToLoad = File.ReadAllLines(_queueFilePath);
            foreach (string resource in resourcesToLoad)
            {
                if (resource.StartsWith("!"))
                {
                    _doNotLoad.Add(resource.Substring(1).ToLower());
                }
                else
                {
                    string[] splitResource = resource.ToLower().Split('.');
                    AddResourceToQueue(splitResource[0], splitResource[1]);
                }
            }
            LoadResources();
            if (_assetTable.ContainsKey("dlg"))
            {
                foreach (IEResRef dlgRef in _assetTable["dlg"].Values)
                {
                    DLG dlgAsset = (DLG)dlgRef.LoadedAsset;
                    dlgAsset.ReplaceDReferences();
                }
            }
            GenerateTP2();
            MasterTRA.WriteTRA(_postConversionDirectory);
        }
        private static void GenerateTP2()
        {
            string output = "";
            foreach(string key in _assetTable.Keys)
            {
                output += "//" + key + " files" + Environment.NewLine;
                output += "//===================" + Environment.NewLine;
                Dictionary<string, IEResRef> innerTable = _assetTable[key];
                foreach(IEResRef resRef in innerTable.Values)
                {
                    output += resRef.LoadedAsset.ToTP2String();
                }
                output += Environment.NewLine;
            }
            string mosDirectory = _postConversionDirectory + "mos\\";
            string bmpDirectory = _postConversionDirectory + "bmp\\";
            if (Directory.Exists(mosDirectory))
            {
                DirectoryContentsToTP2(mosDirectory, "mos", ref output);
            }
            if(Directory.Exists(bmpDirectory))
            {
                DirectoryContentsToTP2(bmpDirectory, "bmp", ref output);
            }
            File.WriteAllText(_postConversionDirectory + "generated.tp2", output);
        }
        private static void DirectoryContentsToTP2(string dirPath, string fileType, ref string tp2output)
        {
            string[] files = Directory.GetFiles(dirPath);
            if(files.Length > 0)
            {
                tp2output += Environment.NewLine + "//" + fileType + " files." + Environment.NewLine;
                tp2output += "//=========================" + Environment.NewLine;
            }
            foreach (string file in files)
            {
                string[] split = file.Split("\\");
                tp2output += _modFolder + split[split.Length - 2] + "\\" + split[split.Length - 1] + Environment.NewLine;
            }
        }
        public static void RegisterDialog(string oldName, string newName)
        {
            _dialogRegistry.Add(oldName, newName);
        }

        public static bool GetNewDialogReference(string oldName, ref string newName)
        {
            string toLower = oldName.ToLower();
            if (_dialogRegistry.ContainsKey(toLower))
            {
                newName = _dialogRegistry[toLower];
                return true;
            }
            else
            {
                return false;
            }
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
                //Console.WriteLine("Loading reference " + resRef.OldReferenceID + "." + resRef.ResourceType);
                newlyLoaded.Add(resRef.OldReferenceID);
                string assetPath = _preConversionDirectory + resRef.ResourceType + "\\" + resRef.OldReferenceID + "." + resRef.ResourceType;
                string postConversionPath = _postConversionDirectory + resRef.ResourceType + "\\" + resRef.NewReferenceID + "." + resRef.ResourceType;
                IEAsset loadedAsset = null;
                if (File.Exists(assetPath))
                {
                    switch (resRef.ResourceType)
                    {
                        case "are":
                            //Console.WriteLine("... Resource is an area.");
                            loadedAsset = new ARE(assetPath, postConversionPath, resRef);
                            if (!Directory.Exists(_postConversionDirectory + "bmp"))
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
                            foreach (string bmp in areaBMPs)
                            {
                                if (File.Exists(bmp))
                                {
                                    File.Copy(bmp, _postConversionDirectory + "bmp\\" + resRef.NewReferenceID + bmp.Substring(bmp.Length - 6, 6), true);
                                }
                            }
                            if (!Directory.Exists(_postConversionDirectory + "mos"))
                            {
                                Directory.CreateDirectory(_postConversionDirectory + "mos");
                            }
                            List<string> areaMOS = new List<string>()
                            {
                                _preConversionDirectory + "mos\\" + resRef.OldReferenceID + ".mos",
                                _preConversionDirectory + "mos\\" + resRef.OldReferenceID + "n.mos",
                            };
                            foreach (string mos in areaMOS)
                            {
                                if (File.Exists(mos))
                                {
                                    if (mos.EndsWith("n.mos"))
                                    {
                                        File.Copy(mos, _postConversionDirectory + "mos\\" + resRef.NewReferenceID + "n.mos", true);
                                    }
                                    else
                                    {
                                        File.Copy(mos, _postConversionDirectory + "mos\\" + resRef.NewReferenceID + ".mos", true);
                                    }
                                }
                            }
                            break;
                        case "baf":
                        case "bam":
                        case "tis":
                        case "wed":
                        case "wav":
                            loadedAsset = new IEAsset(assetPath, postConversionPath, resRef);
                            break;
                        case "itm":
                            loadedAsset = new ITM(assetPath, postConversionPath, resRef);
                            break;
                        case "dlg":
                            loadedAsset = new DLG(assetPath, postConversionPath, resRef);
                            break;
                        case "bmp":
                            //loadedAsset = new BMP(_preConversionDirectory, resRef.ResourceType);
                            break;
                        case "cre":
                            loadedAsset = new CRE(assetPath, postConversionPath, resRef);
                            break;
                    }
                }
                if(loadedAsset != null)
                {
                    resRef.LoadedAsset = loadedAsset;
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
        private static byte[] GetReplacementResource(string resourceID, string resourceType)
        {
            return _assetTable[resourceType][resourceID].ReferenceBytes;
        }
        public static bool IsInQueue(string resource)
        {
            return _resourceQueue.ContainsKey(resource);
        }

        public static byte[] AddResourceToQueue(string resourceID, string resourceType)
        {
            if (_doNotLoad.Contains(resourceID.ToLower() + "." + resourceType.ToLower()))
            {
                return new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            }
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
