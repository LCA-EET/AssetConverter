using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
namespace AssetConverter
{
    internal class Program
    {
        private static Dictionary<string, string> _nameConversion;
        //private static int assetID = 100000;
        private static string _postConversionPath;
        private static string _weiduDirectory;
        private static string _weiduPath;
        private static Dictionary<int, int> _idTable;
        private static Regex _referenceRegex;
        private static int _nextID;
        static void Main(string[] args)
        {
            _referenceRegex = new Regex(@"[a-zA-Z0-9_]{3,8}", RegexOptions.Compiled);
            _nameConversion = new Dictionary<string, string>();
            _idTable = new Dictionary<int, int>();
            _nextID = 100000;
            _idTable.Add(3, 0);
            _idTable.Add(4, 0);
            _idTable.Add(5, 0);
            _idTable.Add(6, 0);
            _idTable.Add(7, 0);
            _idTable.Add(8, 0);
            /*
            string paramFile = args[0];
            string[] paramLines = File.ReadAllLines(paramFile);
            */

            //string conversionPath = paramLines[0];

            string conversionPath = @"C:\Users\dan_v\OneDrive\Desktop\AssetConverter";
            string preconversionpath = conversionPath + @"\preconvert\";
            _postConversionPath = conversionPath + @"\postconvert\";
            _weiduDirectory = @"F:\BGModding - LCA\Game\00766\";
            _weiduPath = _weiduDirectory + "weidu.exe";

            if (Directory.Exists(_postConversionPath))
            {
                Directory.Delete(_postConversionPath, true);
                Directory.CreateDirectory(_postConversionPath);
            }
            RenameAndCopy(preconversionpath + "baf", "baf");
            RenameAndCopy(preconversionpath + "tis", "tis");
            RenameAndCopy(preconversionpath + "wed", "wed");
            UpdateComponentReferences(_postConversionPath + "wed");
            DLGtoDConversion(preconversionpath + "dlg");
            /*
            string[] preconversionDirectories = Directory.GetDirectories(preconversionpath);

            

            foreach (string toProcess in preconversionDirectories)
            {
                string assetType = toProcess.Replace(preconversionpath, "").ToLower();

                // atomic types first, then complex (types that reference other types)

                RenameAndCopy(toProcess, assetType);

                switch (assetType)
                {
                    case "are":

                        break;
                    case "baf":
                        RenameAndCopy(toProcess, assetType);
                        break;
                    case "bmp":

                        break;
                    case "dlg":
                        DLGtoDConversion(toProcess);
                        break;
                    case "tis":
                        RenameAndCopy(toProcess, assetType);
                        break;
                    case "wed":

                        break;
                }
                    
            }
            */
            string output = "";
            foreach(string oldName in _nameConversion.Keys)
            {
                output += oldName + "," + _nameConversion[oldName] + Environment.NewLine;
            }
            File.WriteAllText("referenceTable.txt", output);
        }
         
        static string GetName(string filePath)
        {
            string[] splitPath = filePath.Split(@"\");
            string fileName = splitPath[splitPath.Length-1].Split(".")[0];
            return fileName.Split(".")[0].ToLower();
        }
        static void DLGtoDConversion(string assetDirectory)
        {
            string[] dlgFiles = Directory.GetFiles(assetDirectory);
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
            RenameAndCopy(assetDirectory, "d");
            RenameAndCopy(assetDirectory, "tra");
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
                            List<byte> toConvert = new List<byte>();
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
                                input.InsertRange(i, Encoding.Latin1.GetBytes(replacement.ToUpper()));
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
        static int GetNextID()
        {
            int toReturn = _nextID;
            _nextID++;
            return toReturn;
        }
        static void RenameAndCopy(string assetDirectory, string assetType)
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
                    int nextID = -1;
                    if (_nameConversion.ContainsKey(assetName))
                    {
                        newName = _nameConversion[assetName];
                        File.Copy(assetToProcess, destinationDirectory + @"\" + newName + "." + assetType);
                    }
                    else
                    {
                        if (_idTable.ContainsKey(assetName.Length))
                        {
                            /*
                            //nextID = _idTable[assetName.Length];
                            //_idTable[assetName.Length] = nextID + 1;
                            string toAppend = nextID.ToString();
                            while(toAppend.Length < assetName.Length - 2)
                            {
                                toAppend = "0" + toAppend;
                            }
                            */
                            newName = "xa" + GetNextID().ToString();
                            _nameConversion.Add(assetName, newName);
                            File.Copy(assetToProcess, destinationDirectory + @"\" + newName + "." + assetType, true);
                        }
                    }
                }
            }
        }
    }
}
