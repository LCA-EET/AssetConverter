﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
namespace AssetConverter
{
    public static class ResourceManager
    {
        private static Dictionary<string, string> _pvrzTable;
        private static Dictionary<string, string> _scriptNames;
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
        private static Dictionary<string, int> _nextIDTable;
        
        //private static int _nextID;
        
        public static void Initialize(string preConversionDirectory, string postConversionDirectory, string queueFilePath, string weiduPath, string filePrefix, string modFolder)
        {

            //_nextID = 1000;
            _pvrzTable = new Dictionary<string, string>();
            _nextIDTable = new Dictionary<string, int>();
            _scriptNames = new Dictionary<string, string>();   
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
        public static void AddPVRZ(string oldPath, string newPath)
        {
            if (!_pvrzTable.ContainsKey(oldPath))
            {
                _pvrzTable.Add(oldPath, newPath);
            }
        }
        public static void RegisterRevisedScriptName(string oldName, string newName)
        {
            if (!_scriptNames.ContainsKey(oldName.ToUpper()))
            {
                _scriptNames.Add(oldName.ToUpper(), newName.ToUpper());
            }
        }
        public static string ReadString_Latin1(byte[] contents, int start, int length)
        {
            List<byte> toProcess = new List<byte>();
            int index = 0;
            while(index < length)
            {
                byte toCheck = contents[start + index];
                if(toCheck == 0x00)
                {
                    break;
                }
                else
                {
                    toProcess.Add(toCheck);
                }
                index++;
            }
            return Encoding.Latin1.GetString(toProcess.ToArray());
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
                string resourceLower = resource.ToLower();
                if (resourceLower.StartsWith("!"))
                {
                    _doNotLoad.Add(resourceLower.Substring(1));
                }
                else
                {
                    if(resourceLower.Trim() != "")
                    {
                        try
                        {
                            string[] splitResource = resourceLower.Split('.');
                            AddResourceToQueue(splitResource[0], splitResource[1], false);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("ERROR: " + resourceLower);
                        }
                    }
                }
            }
            LoadResources();
            
            GenerateTP2();
            
            MasterTRA.WriteTRA();
        }
        private static void GenerateTP2()
        {
            string refoutput = "";
            string output = "PRINT ~Processing converted asset block: " + Program.paramFile.Prefix + "...~" + Environment.NewLine;
            output += "SILENT" + Environment.NewLine;
            if (MusicTable.SongCount > 0)
            {
                File.WriteAllText(_postConversionDirectory + "songsList.txt", MusicTable.TableToString());
                output += MusicTable.ToTP2String();
            }
            foreach (string key in _assetTable.Keys)
            {
                if(key == "dlg" && _assetTable[key].Count > 0)
                {
                    //output += "PRINT ~Compiling dialogs...~" + Environment.NewLine;
                    output += "COMPILE EVALUATE_BUFFER ~" + Program.paramFile.ModFolder + "d~" + Environment.NewLine;
                }
                //output += "//" + key + " files" + Environment.NewLine;
                //output += "//===================" + Environment.NewLine;
                Dictionary<string, IEResRef> innerTable = _assetTable[key];
                if(key == "baf" && _assetTable[key].Count > 0)
                {
                    Dictionary<string, IEResRef> bafAssets = _assetTable[key];
                    foreach(IEResRef assetRef in bafAssets.Values)
                    {
                        ((BAF)(assetRef.LoadedAsset)).PerformPostProcessing();
                    }

                    //output += "PRINT ~Compiling scripts...~" + Environment.NewLine;
                    
                }
                // output += "PRINT ~Processing " + key + " files...~" + Environment.NewLine;
                HashSet<string> bulkCopy = new HashSet<string>() { "bam", "dlg", "baf", "wav", "pvrz", "tis", "wed" };
                foreach (IEResRef resRef in innerTable.Values)
                {
                    //output += "PRINT ~Processing " + resRef.NewReferenceID +"." + resRef.ResourceType +"~" + Environment.NewLine;
                    refoutput += resRef.OldReferenceID + "." + resRef.ResourceType + "::" + resRef.NewReferenceID + "." + resRef.ResourceType +  Environment.NewLine;

                    if (!bulkCopy.Contains(resRef.ResourceType))
                    {
                        output += resRef.LoadedAsset.ToTP2String();
                    }
                    
                }
                output += Environment.NewLine;
                
            }
            if (_pvrzTable.Count > 0)
            {
                if(!Directory.Exists(_postConversionDirectory + "pvrz"))
                {
                    Directory.CreateDirectory(_postConversionDirectory + "pvrz");
                    foreach (string pvrzFile in _pvrzTable.Keys)
                    {
                        File.Copy(pvrzFile, _pvrzTable[pvrzFile]);
                    }
                    /*
                    output += "//{ PVRZ" + Environment.NewLine; ;
                    
                    foreach(string pvrzFile in _pvrzTable.Keys)
                    {
                        File.Copy(pvrzFile, _pvrzTable[pvrzFile]);
                        
                        output += "\tCOPY ~" + Program.paramFile.ModFolder + "pvrz\\" + Path.GetFileName(_pvrzTable[pvrzFile]) + "~ ~override~" + Environment.NewLine; 
                    }
                    output += "//}" + Environment.NewLine;
                    */
                }
            }
            string[] componentDirs = { "bam", "bmp", "mos", "pvrz", "tis", "wav", "wed" };
            for(int i = 0; i < componentDirs.Length; i++)
            {
                string dirPath = _postConversionDirectory + componentDirs[i];
                if (Directory.Exists(dirPath))
                {
                    DirectoryContentsToTP2(dirPath, componentDirs[i], ref output);
                }
            }
            string bafDirectory = _postConversionDirectory + "baf";
            if (Directory.Exists(bafDirectory))
            {
                if(Directory.GetFiles(bafDirectory).Length > 0)
                {
                    output += "COMPILE EVALUATE_BUFFER ~" + Program.paramFile.ModFolder + "baf~" + Environment.NewLine;
                }
            }
            File.WriteAllText(_postConversionDirectory + "generated.tph", output);
            //File.WriteAllText(_postConversionDirectory + "referenceTable.txt", refoutput);
            //File.Copy(Program.paramFile.QueuePath, _postConversionDirectory + "queue.txt");
            //File.Copy(Program.ParamPath, _postConversionDirectory + "params.in");
            
            AssetRegister.WriteRegister();
        }
        private static void DirectoryContentsToTP2(string dirPath, string fileType, ref string tp2output)
        {
            string[] files = Directory.GetFiles(dirPath);
            if(files.Length > 0)
            {
                tp2output += Environment.NewLine + "//" + fileType + " files." + Environment.NewLine;
                tp2output += "//=========================" + Environment.NewLine;
            }
            /*
            foreach (string file in files)
            {
                string[] split = file.Split("\\");
                //tp2output += "ACTION_IF (!FILE_EXISTS_IN_GAME " + filename + ") BEGIN" + Environment.NewLine; 
                tp2output += "COPY ~" + _modFolder + split[split.Length - 2] + "\\" + split[split.Length - 1] + "~ ~override~" + Environment.NewLine;
                //tp2output += "END";
            }
            */
            tp2output += "COPY ~" + _modFolder + fileType + "~ ~override~" + Environment.NewLine;
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
        private static void LoadAsset(string assetPath, string postConversionPath, IEResRef resRef)
        {
            IEAsset loadedAsset = null;
            if (File.Exists(assetPath))
            {
                switch (resRef.ResourceType)
                {
                    case "are":
                        loadedAsset = new ARE(assetPath, postConversionPath, resRef);
                        break;
                    case "baf":
                        loadedAsset = new BAF(assetPath, postConversionPath, resRef);
                        break;
                    case "bam":
                        loadedAsset = new BAM(assetPath, postConversionPath, resRef);
                        break;
                    case "bmp":
                    case "eff":
                        loadedAsset = new IEAsset(assetPath, postConversionPath, resRef);
                        break;
                    case "tis":
                        loadedAsset = new TIS(assetPath, postConversionPath, resRef);
                        break;
                    case "wav":
                        if (Program.paramFile.IncludeWAVs)
                        {
                            loadedAsset = new IEAsset(assetPath, postConversionPath, resRef);
                        }
                        break;
                    case "wed":
                        loadedAsset = new WED(assetPath, postConversionPath, resRef);
                        if (!Directory.Exists(_postConversionDirectory + "bmp"))
                        {
                            Directory.CreateDirectory(_postConversionDirectory + "bmp");
                        }
                        List<string> wedBMPs = new List<string>()
                                {
                                    _preConversionDirectory + "bmp\\" + resRef.OldReferenceID + "ht.bmp",
                                    _preConversionDirectory + "bmp\\" + resRef.OldReferenceID + "lm.bmp",
                                    _preConversionDirectory + "bmp\\" + resRef.OldReferenceID + "ln.bmp",
                                    _preConversionDirectory + "bmp\\" + resRef.OldReferenceID + "sr.bmp",
                                };
                        foreach (string bmp in wedBMPs)
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
                        string wedMOSPath = _preConversionDirectory + "mos\\" + resRef.OldReferenceID + ".mos";

                        if (File.Exists(wedMOSPath))
                        {
                            File.Copy(wedMOSPath, _postConversionDirectory + "mos\\" + resRef.NewReferenceID + ".mos", true);
                        }
                        break;
                    case "spl":
                        loadedAsset = new SPL(assetPath, postConversionPath, resRef);
                        break;
                    case "sto":
                        loadedAsset = new STO(assetPath, postConversionPath, resRef);
                        break;
                    case "itm":
                        loadedAsset = new ITM(assetPath, postConversionPath, resRef);
                        break;
                    case "dlg":
                        loadedAsset = new DLG(assetPath, postConversionPath, resRef);
                        break;
                    case "cre":
                        loadedAsset = new CRE(assetPath, postConversionPath, resRef);
                        break;
                }
            }
            if (loadedAsset != null)
            {
                resRef.LoadedAsset = loadedAsset;
                _assetTable[resRef.ResourceType].Add(resRef.OldReferenceID, resRef);
            }
        }
        private static void LoadResources()
        {
            string toLoad;
            List<string> newlyLoaded = new List<string>();
            List<IEResRef> toProcess = [.. _resourceQueue.Values];
            foreach (IEResRef resRef in toProcess)
            {
                newlyLoaded.Add(resRef.OldReferenceID + "." + resRef.ResourceType);
                
                if (resRef.SkipLoad == false)
                {
                    //Console.WriteLine("Loading reference " + resRef.OldReferenceID + "." + resRef.ResourceType);

                    string assetPath = _preConversionDirectory + resRef.RelativeAssetPath;
                    string postConversionPath = _postConversionDirectory + resRef.RelativeConversionPath;

                    LoadAsset(assetPath, postConversionPath, resRef);
                    
                    if (resRef.IsNightType)
                    {

                        IEResRef nightRef = new IEResRef(resRef.OldReferenceID + "n", resRef.ResourceType, false, PadBytes(Encoding.Latin1.GetBytes(resRef.NewReferenceID + "n").ToList()));
                        newlyLoaded.Add(nightRef.OldReferenceID + "." + nightRef.ResourceType);
                        string nightAssetPath = _preConversionDirectory + nightRef.RelativeAssetPath;
                        string nightConversionPath = _postConversionDirectory + nightRef.RelativeConversionPath;
                        LoadAsset(nightAssetPath, nightConversionPath, nightRef);
                    }
                    
                }
                else
                {
                    Console.WriteLine("Asset skipped: " + resRef.OldReferenceID + "." + resRef.ResourceType);
                    //Console.ReadLine();
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
                if (_assetTable.ContainsKey("dlg"))
                {
                    foreach (IEResRef dlgRef in _assetTable["dlg"].Values)
                    {
                        DLG dlgAsset = (DLG)dlgRef.LoadedAsset;
                        if (!dlgAsset.Processed)
                        {
                            dlgAsset.ReplaceDReferences();
                        }
                    }
                }
                if (_resourceQueue.Count > 0)
                {
                    LoadResources();
                }
                foreach (string assetType in _assetTable.Keys) 
                {
                    if (!Directory.Exists(_postConversionDirectory + assetType))
                    {
                        Directory.CreateDirectory(_postConversionDirectory + assetType);
                    }
                    Dictionary<string, IEResRef> loaded = _assetTable[assetType];
                    foreach (IEResRef res in loaded.Values)
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
        private static int GetNextID(string resourceType)
        {
            int toReturn;
            if (!_nextIDTable.ContainsKey(resourceType))
            {
                if(resourceType == "wav")
                {
                    _nextIDTable.Add(resourceType, Program.paramFile.FirstWAVID);
                }
                else
                {
                    _nextIDTable.Add(resourceType, Program.paramFile.FirstID);
                }
                
            }
            toReturn = _nextIDTable[resourceType];
            _nextIDTable[(resourceType)] = toReturn + 1;
            return toReturn;
        }
        private static byte[] PadBytes(List<byte> referenceBytes)
        {
            while (referenceBytes.Count < 8)
            {
                referenceBytes.Add(0x00);
            }
            return referenceBytes.ToArray();
        }
        private static byte[] GetNextResourceID(string resourceType)
        {
            string toReturn = _filePrefix.ToLower() + GetNextID(resourceType);
            return PadBytes(Encoding.Latin1.GetBytes(toReturn).ToList());

        }
        public static byte[] AddResourceToQueue(string resourceID, string resourceType, byte[] nextResourceID, bool skipLoad)
        {
            if(nextResourceID == null)
            {
                nextResourceID = GetNextResourceID(resourceType);
            }
            resourceID = resourceID.ToLower();
            resourceType = resourceType.ToLower();
            string resourceIDandType = resourceID + "." + resourceType;
            if (_doNotLoad.Contains(resourceIDandType))
            {
                //Console.WriteLine("Skipped loading: " + resourceIDandType);
                //Console.Read();
                return new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            }
            byte[] importedReference = null;
            if(AssetRegister.AlreadyImported(resourceType, resourceID, ref importedReference))
            {
                return importedReference;
            }
            else
            {
                if (!_resourceQueue.ContainsKey(resourceIDandType))
                {
                    if (!_assetTable.ContainsKey(resourceType))
                    {
                        _assetTable.Add(resourceType, new Dictionary<string, IEResRef>());
                    }
                    if (!_assetTable[resourceType].ContainsKey(resourceID))
                    {
                        IEResRef toAdd = new IEResRef(resourceID, resourceType, skipLoad, nextResourceID);
                        _resourceQueue.Add(resourceIDandType, toAdd);
                        
                        return toAdd.ReferenceBytes;
                    }
                    else
                    {
                        return _assetTable[resourceType][resourceID].ReferenceBytes;
                    }
                }
                else
                {
                    return _resourceQueue[resourceIDandType].ReferenceBytes;
                }
            }
        }
        
        public static byte[] AddResourceToQueue(string resourceID, string resourceType, bool skipLoad)
        {
            return AddResourceToQueue(resourceID, resourceType, GetNextResourceID(resourceType), skipLoad);
        }

    }
}
