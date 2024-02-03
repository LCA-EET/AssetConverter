using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
namespace AssetConverter
{
    internal class Program
    {
        private static string _queuePath;
        private static string _preConversionPath;
        private static string _postConversionPath;
        private static string _weiduDirectory;
        private static string _weiduPath;
        
        private static string _filePrefix;

        public static MasterTRA MasterTRA;
        static void Main(string[] args)
        {
            _filePrefix = "xa";
            
            //string conversionPath = paramLines[0];
            string conversionPath = @"F:\AssetConverter";
            _preConversionPath = conversionPath + @"\preconvert\";
            _postConversionPath = conversionPath + @"\postconvert\";
            if(Directory.Exists(_postConversionPath))
            {
                Directory.Delete(_postConversionPath, true);
            }
            Directory.CreateDirectory(_postConversionPath);
            _weiduDirectory = @"F:\BGModding - LCA\Game\00766\";
            _weiduPath = _weiduDirectory + "weidu.exe";
            _queuePath = "queue.txt";

            WeiduGenerateTLK();
            ResourceManager.Initialize(_preConversionPath,
                _postConversionPath,
                _queuePath,
                _weiduPath,
                _filePrefix
                );
            /*
            ProcessDirectory("bmp");
            ProcessDirectory("baf");
            ProcessDirectory("tis");
            ProcessDirectory("wed");
            //ProcessDirectory("dlg");
            //ProcessDirectory("dlg\\d");
            //ProcessDirectory("dlg\\tra");
            ProcessDirectory("itm");
            ProcessDirectory("wav");
            ProcessDirectory("cre");
            ProcessDirectory("are");
            */
        }
        static void WeiduGenerateTLK()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(_weiduPath);
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = _weiduDirectory;
            startInfo.ArgumentList.Add("--traify-tlk");
            startInfo.ArgumentList.Add("--out");
            startInfo.ArgumentList.Add("text.tra");
            Process conversion = Process.Start(startInfo);
            conversion.WaitForExit();
            MasterTRA = new MasterTRA(_weiduDirectory + "text.tra");
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
                    //BuildAsset(files[i]);
                }
            }
            Log.WriteLineToLog("Done.");
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
