using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
namespace AssetConverter
{
    internal class Program
    {

        private static string _preConversionPath;
        private static string _postConversionPath;
        private static string _weiduDirectory;
        private static string _weiduPath;
        
        private static IEAssetTable _assetTable;
        private static string _filePrefix;

        public static ReferenceGenerator ReferenceTable;
        static void Main(string[] args)
        {
            _filePrefix = "xa";
            
            //string conversionPath = paramLines[0];
            string conversionPath = @"F:\AssetConverter";
            _preConversionPath = conversionPath + @"\preconvert\";
            _postConversionPath = conversionPath + @"\postconvert\";
            _weiduDirectory = @"F:\BGModding - LCA\Game\00766\";
            _weiduPath = _weiduDirectory + "weidu.exe";

            ReferenceTable = new ReferenceGenerator(_filePrefix, _postConversionPath);
            if (Directory.Exists(_postConversionPath))
            {
                Directory.Delete(_postConversionPath, true);
            }
            if (Directory.Exists(_preConversionPath + "dlg"))
            {
                RemoveDLGComponents(_preConversionPath + "dlg");
            }
            ProcessDirectory("bmp");
            ProcessDirectory("baf");
            ProcessDirectory("tis");
            ProcessDirectory("wed");
            ProcessDirectory("dlg");
            ProcessDirectory("dlg\\d");
            ProcessDirectory("dlg\\tra");
            ProcessDirectory("itm");
            ProcessDirectory("wav");
            ReferenceTable.SaveAssetsPostConversion();
            /*
            Console.WriteLine("Processing BAF");
            RenameAndCopy(preconversionpath + "baf", "baf", 1);
            Console.WriteLine("Processing TIS");
            RenameAndCopy(preconversionpath + "tis", "tis", 1);
            Console.WriteLine("Processing WED");
            RenameAndCopy(preconversionpath + "wed", "wed", 1);
            Console.WriteLine("Updating WED Component References");
            UpdateComponentReferences(_postConversionPath + "wed");
            Console.WriteLine("Processing DLG");
            DLGtoDConversion(preconversionpath, 1);
            Console.WriteLine("Processing BMP");
            RenameBMPandCopy(preconversionpath, 1);
            Console.WriteLine("Processing Area Ambient WAVs");
            RenameAmbientsAndCopy(preconversionpath, 2);
            Console.WriteLine("Processing ARE");
            RenameAndCopy(preconversionpath + "are", "are", 1);
            Console.WriteLine("Updating ARE Component References");
            UpdateComponentReferences(_postConversionPath + "are");
            string output = "";
            /*
            foreach(string oldName in _nameConversion.Keys)
            {
                output += oldName + "," + _nameConversion[oldName] + Environment.NewLine;
            }
            
            File.WriteAllText("referenceTable.txt", output);
            */
        }
        static void RemoveDLGComponents(string dlgDirectory)
        {
            if(Directory.Exists(dlgDirectory + "\\d"))
            {
                Directory.Delete(dlgDirectory + "\\d", true);
            }
            if (Directory.Exists(dlgDirectory + "\\tra"))
            {
                Directory.Delete(dlgDirectory + "\\tra", true);
            }
        }
        static void ProcessDirectory(string directory)
        {
            Log.WriteLineToLog("Processing files in " + _preConversionPath + directory + "...\n");
            if(Directory.Exists(_preConversionPath + directory))
            {
                string[] files = Directory.GetFiles(_preConversionPath + directory);
                for (int i = 0; i < files.Length; i++)
                {
                    Log.WriteToLog((i + 1) + " of " + files.Length);
                    BuildAsset(files[i]);
                }
            }
            Log.WriteLineToLog("Done.");
        }
        static void BuildAsset(string assetPath)
        {
            string assetType = assetPath.Split(".")[1].ToLower();
            IEAsset toAdd = null;
            switch (assetType)
            {
                case "baf":
                case "tis":
                case "wed":
                case "tra":
                    toAdd = new IEAsset(assetPath, assetType);
                    break;
                case "dlg":
                    toAdd = new DLG(assetPath, assetType, _weiduPath);
                    break;
                case "d":
                    toAdd = new D(assetPath, assetType);
                    break;
                case "bmp":
                    toAdd = new BMP(assetPath, assetType);
                    break;
                case "wav":
                    WAV wavAsset = new WAV(assetPath, assetType);
                    if (wavAsset.IsAmbient)
                    {
                        toAdd = wavAsset;
                    }
                    break;

            }
            if(toAdd != null)
            {
                ReferenceTable.AssociateAssetToReference(toAdd);
            }
        }
        /*
            static void RenameAmbientsAndCopy(string assetDirectory, int idIndex)
            {
                assetDirectory += "wav";
                string[] wavFiles = Directory.GetFiles(assetDirectory);
                string destinationDirectory = _postConversionPath + "wav";
                Directory.CreateDirectory(destinationDirectory);
                for (int i = 0; i < wavFiles.Count(); i++ )
                {
                    string wavLower = GetName(wavFiles[i]).ToLower();
                    string newName = "";
                    if (wavLower.StartsWith("am"))
                    {
                        if (_nameConversion.ContainsKey(wavLower))
                        {
                            newName = _nameConversion[wavLower];
                            File.Copy(wavFiles[i], destinationDirectory + @"\" + newName + ".wav");
                        }
                        else
                        {
                            newName = _filePrefix + GetNextID(idIndex).ToString();
                            _nameConversion.Add(wavLower, newName);
                            File.Copy(wavFiles[i], destinationDirectory + @"\" + newName + ".wav");
                        }
                    }
                }
            }

            static void RenameBMPandCopy(string assetDirectory, int idIndex)
            {
                assetDirectory += "bmp";
                string[] bmpFiles = Directory.GetFiles(assetDirectory);
                string destinationDirectory = _postConversionPath + "bmp";
                Directory.CreateDirectory(destinationDirectory);
                for (int i = 0; i < bmpFiles.Length; i++)
                {
                    string bmpLower = GetName(bmpFiles[i]).ToLower();
                    string newName = "";
                    if (bmpLower.Length == 8)
                    {
                        string suffix = bmpLower.Substring(6, 2);
                        Console.WriteLine("... Suffix: " + suffix);
                        string firstSix = bmpLower.Substring(0, 6);
                        Console.WriteLine("... First Six: " + firstSix);
                        switch (suffix)
                        {
                            case "ht":
                            case "lm":
                            case "ln":
                            case "sr":
                                if (_nameConversion.ContainsKey(firstSix))
                                {
                                    newName = _nameConversion[firstSix] + suffix + ".bmp";
                                }
                                else
                                {
                                    newName = _filePrefix + GetNextID(idIndex).ToString() + ".bmp";
                                    _nameConversion.Add(bmpLower, newName);
                                }
                                File.Copy(bmpFiles[i], destinationDirectory + @"\" + newName);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        if (_nameConversion.ContainsKey(bmpLower))
                        {
                            newName = _nameConversion[bmpLower] + ".bmp";
                            File.Copy(bmpFiles[i], destinationDirectory + @"\" + newName);
                        }
                        else
                        {
                            newName = _filePrefix + GetNextID(idIndex).ToString() + ".bmp";
                            _nameConversion.Add(bmpLower, newName);
                            File.Copy(bmpFiles[i], destinationDirectory + @"\" + newName);
                        }
                    }
                }
            }
            static string GetName(string filePath)
            {
                string[] splitPath = filePath.Split(@"\");
                string fileName = splitPath[splitPath.Length-1].Split(".")[0];
                return fileName.Split(".")[0].ToLower();
            }
            static void DLGtoDConversion(string assetDirectory, int idIndex)
            {
                //assetDirectory += "dlg";
                string[] dlgFiles = Directory.GetFiles(assetDirectory + "dlg");
                ProcessStartInfo startInfo = new ProcessStartInfo(_weiduPath);
                startInfo.UseShellExecute = false;
                startInfo.WorkingDirectory= assetDirectory;
                foreach (string file in dlgFiles)
                {
                    if (file.ToLower().EndsWith(".dlg"))
                    {
                        startInfo.ArgumentList.Clear();
                        startInfo.ArgumentList.Add("--game");
                        startInfo.ArgumentList.Add(_weiduDirectory);
                        startInfo.ArgumentList.Add("--trans");
                        startInfo.ArgumentList.Add(file);
                        Process conversion = Process.Start(startInfo);
                        conversion.WaitForExit();
                    }
                }
                RenameAndCopy(assetDirectory + "dlg", "d", idIndex);
                RenameAndCopy(assetDirectory + "dlg", "tra", idIndex);
                ReplaceDReferences(_postConversionPath + @"d\");
            }
            static void ReplaceDReferences(string dFileDirectory)
            {
                string beginFlag = "BEGIN ~";
                string externFlag = "EXTERN ~";
                string[] dFiles = Directory.GetFiles(dFileDirectory);
                foreach(string dFile in dFiles)
                {
                    bool changeMade = false;
                    string[] lineContents = File.ReadAllLines(dFile);
                    for (int i = 0; i < lineContents.Length; i++)
                    {
                        string reference = "";
                        if (lineContents[i].StartsWith(beginFlag))
                        {
                            reference = lineContents[i].Split(beginFlag)[1].ToLower();
                        }
                        if (lineContents[i].Contains(externFlag))
                        {
                            reference = lineContents[i].Split(externFlag)[1].ToLower();
                        }
                        if (reference != "")
                        {
                            reference = reference.Split("~")[0];
                            LogToConsole("D reference found: " + reference);
                            if (_nameConversion.ContainsKey(reference))
                            {
                                lineContents[i] = lineContents[i].Replace(reference.ToUpper(), _nameConversion[reference].ToUpper());
                                LogToConsole("Replaced with: " + _nameConversion[reference]);
                                changeMade = true;
                            }
                        }

                    }
                    if (changeMade)
                    {
                        File.WriteAllLines(dFile, lineContents);
                    }
                }

            }

            static bool IsCharacterByte(byte toCheck)
            {
                if(toCheck >= 30 && toCheck <= 39)
                {
                    //is a digit
                    return true;
                }
                else if (toCheck >= 65 && toCheck <= 90)
                {
                    //is a capital letter
                    return true;
                }
                else if (toCheck >= 97 && toCheck <= 122)
                {
                    //is a capital letter
                    return true;
                }
                else if (toCheck == (byte)95)
                {
                    //is an underscore
                    return true;
                }
                else if(toCheck == (byte)35)
                {
                    return true;
                }
                return false;
            }
            static void UpdateComponentReferences(string directoryPath)
            {
                string[] filesToProcess = Directory.GetFiles(directoryPath);
                foreach(string filePath in filesToProcess)
                {
                    LogToConsole("Replacing component references in " + filePath);//
                    List<byte> input = File.ReadAllBytes(filePath).ToList();
                    for(int i = 0; i < input.Count; i++)
                    {
                        if (IsCharacterByte(input[i]))
                        {
                            if(i + 7 <= (input.Count - 1))
                            {
                                List<byte> nullRemoved = input.GetRange(i, 8);
                                for (int j = nullRemoved.Count-1; j >= 0; j--)
                                {
                                    if (nullRemoved[j] == 0)
                                    {
                                        nullRemoved.RemoveAt(j);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                string toCheck = Encoding.Latin1.GetString(nullRemoved.ToArray()).ToLower().Trim();
                                if (_nameConversion.ContainsKey(toCheck))
                                {
                                    string replacement = _nameConversion[toCheck];
                                    LogToConsole("... Replaced " + toCheck + " with " + replacement);//
                                    input.RemoveRange(i, 8);
                                    input.InsertRange(i, Encoding.Latin1.GetBytes(replacement.ToUpper()).Concat(new byte[] {0,0}));
                                    i += 7;
                                }
                            }
                        }
                    }
                    File.WriteAllBytes(filePath, input.ToArray());
                }

            }
            static void LogToConsole(string toLog)
            {
                Console.WriteLine(toLog);
                File.AppendAllText("log.txt", toLog + Environment.NewLine);
            }
            static int GetNextID(int index)
            {
                int toReturn = 0;
                if (_indexTable.ContainsKey(index))
                {
                    toReturn = _indexTable[index];
                    _indexTable[index] = toReturn + 1;
                }
                else
                {
                    toReturn = 1000;
                    _indexTable.Add(index, 1001);
                }
                return toReturn;
            }
            static void RenameAndCopy(string assetDirectory, string assetType, int idIndex)
            {
                string destinationDirectory = _postConversionPath + assetType;
                Directory.CreateDirectory(destinationDirectory);
                string[] assetFiles = Directory.GetFiles(assetDirectory);

                foreach(string assetToProcess in assetFiles)
                {
                    if (assetToProcess.ToLower().EndsWith(assetType.ToLower()))
                    {
                        string assetName = GetName(assetToProcess).ToLower();
                        string newName;
                        if (_nameConversion.ContainsKey(assetName))
                        {
                            newName = _nameConversion[assetName];
                            File.Copy(assetToProcess, destinationDirectory + @"\" + newName + "." + assetType);
                        }
                        else
                        {
                            newName = _filePrefix + GetNextID(idIndex).ToString();
                            _nameConversion.Add(assetName, newName);
                            File.Copy(assetToProcess, destinationDirectory + @"\" + newName + "." + assetType, true);
                        }
                    }
                }
            }
            */
    }
}
