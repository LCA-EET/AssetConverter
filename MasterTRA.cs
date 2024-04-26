using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public static class MasterTRA
    {
        private static Dictionary<string, int> _addedText;
        private static Dictionary<int, TRARef> _tlkReferences;
        private static Dictionary<int, TRARef> _stringList;
        private static int _nextIndex = 10000;

        public static void InitializeMasterTRA(string tlkFile) 
        {
            if (!File.Exists(Program.paramFile.TRAIndexPath))
            {
                _nextIndex = (int)Program.paramFile.FirstTRAIndex;
            }
            else
            {
                _nextIndex = int.Parse(File.ReadAllText(Program.paramFile.TRAIndexPath));
            }
            _stringList = new Dictionary<int, TRARef>();
            _tlkReferences = new Dictionary<int, TRARef>();
            _addedText = new Dictionary<string, int>();  
            string[] lines = File.ReadAllLines(tlkFile);
            for (int i =0; i < lines.Length; i++)
            {
                //Console.WriteLine("Line: " + i);
                if (lines[i].Contains("@") && lines[i].Contains("="))
                {

                    //string[] eqSplit = lines[i].Split("=");
                    int eqIdx = lines[i].IndexOf("=");

                    int referenceID = int.Parse(lines[i].Substring(0, eqIdx).Trim().Replace("@",""));
                        //eqSplit[0].Replace("@", "").Trim());
                    string toAdd = lines[i].Substring(eqIdx + 1);
                    if(i + 1 < lines.Length)
                    {
                        for (int j = i + 1; j < lines.Length; j++)
                        {
                            if (lines[j].Contains("@"))
                            {
                                AddStringReference(referenceID, toAdd);
                                break;
                            }
                            else
                            {
                                i = i + 1;
                                toAdd += Environment.NewLine + lines[j];
                                
                            }
                        }
                    }
                    else
                    {
                        AddStringReference(referenceID, toAdd);
                    }
                }
            }
        }
        private static void AddStringReference(int referenceID, string toAdd)
        {
            _tlkReferences.Add(referenceID, new TRARef(referenceID, toAdd));
        }
        public static int ConvertToReference(TRARef reference)
        {
            string tlk = reference.TLKString;
            if (_addedText.ContainsKey(tlk))
            {
                return _addedText[tlk];
            }
            else
            {
                reference.ReReference(_nextIndex);
                _stringList.Add(_nextIndex, reference);
                int toReturn = _nextIndex;
                _addedText.Add(tlk, toReturn);
                _nextIndex++;
                return toReturn;
            }
        }
        public static TRARef GetTRAReferenced_Used(int reference)
        {
            if (_stringList.ContainsKey(reference))
            {
                return _stringList[reference];
            }
            return null;
        }
        public static TRARef GetTRAReference_TLK(int reference)
        {
            if (_tlkReferences.ContainsKey(reference))
            {
                return _tlkReferences[reference];
            }
            return null;
        }
        
        public static void WriteTRA()
        {
            string output = "";
            foreach(int key in _stringList.Keys)
            {
                string text = _stringList[key].TLKString;
                if (text == "")
                {
                    text = "~~";
                }
                output += "@" + key + " = " + text + Environment.NewLine;
            }
            //File.WriteAllText(postConversionDirectory + "generated.tra", output);
            File.AppendAllText(Program.paramFile.CombinedTRAPath, output);
            File.WriteAllText(Program.paramFile.TRAIndexPath, _nextIndex.ToString());
        }
    }
}
