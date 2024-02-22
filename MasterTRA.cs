using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public static class MasterTRA
    {
        private static Dictionary<int, TRARef> _tlkReferences;
        private static Dictionary<int, TRARef> _stringList;
        private static int _firstIndex = 10000;
        public static void InitializeMasterTRA(string tlkFile) 
        {
            _firstIndex = (int)Program.paramFile.FirstTRAIndex;
            _stringList = new Dictionary<int, TRARef>();
            _tlkReferences = new Dictionary<int, TRARef>();
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
            /*
            int startIndex = toAdd.IndexOf('~');
            int lastIndex = toAdd.LastIndexOf('~');
            string substring = toAdd.Substring(startIndex, lastIndex - startIndex);
            int startBracket = 0;
            int endBracket = 0;
            if (substring.Contains('[') && substring.Contains(']'))
            {
                startBracket = substring.IndexOf('[');
                endBracket = substring.LastIndexOf(']');
                toAdd = toAdd.Replace(substring.Substring(startBracket, (endBracket - startBracket) + 1), "").Trim();
            }
            string[] split = toAdd.Split("~");
            split[1] = split[1].Trim();
            if (split[2].Contains("["))
            {
                startBracket = split[2].IndexOf('[');
                endBracket = split[2].LastIndexOf(']');
                string wavToReplace = split[2].Substring(startBracket + 1, (endBracket - startBracket) -1);
                byte[] replacementReference = ResourceManager.TrimTrailingNullBytes(ResourceManager.AddResourceToQueue(wavToReplace.ToLower(), "wav"));
                split[2] = "[" + Encoding.Latin1.GetString(replacementReference) + "]";
            }
            toAdd = "~" + split[1] + "~ " + split[2];
            
            _tlkReferences.Add(referenceID, toAdd);
            */
        }
        public static int ConvertToReference(TRARef reference)
        {
            int indexToReturn = _firstIndex + (int)_stringList.Count;
            reference.ReReference(indexToReturn);
            _stringList.Add(indexToReturn, reference);
            
            return indexToReturn;
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
        
        public static void WriteTRA(string postConversionDirectory)
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
            File.WriteAllText(postConversionDirectory + "generated.tra", output);
        }
    }
}
