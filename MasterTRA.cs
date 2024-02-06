using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public static class MasterTRA
    {
        private static Dictionary<uint, string> _tlkReferences;
        private static Dictionary<uint, string> _usedReferences;
        private static Dictionary<uint, string> _stringList;
        private static uint _firstIndex = 10000;
        public static void InitializeMasterTRA(string tlkFile) 
        {
            _stringList = new Dictionary<uint, string>();
            _tlkReferences = new Dictionary<uint, string>();
            _usedReferences = new Dictionary<uint, string>();
            string[] lines = File.ReadAllLines(tlkFile);
            for (int i =0; i < lines.Length; i++)
            {
                //Console.WriteLine("Line: " + i);
                if (lines[i].Contains("@") && lines[i].Contains("="))
                {

                    string[] eqSplit = lines[i].Split("=");
                    uint referenceID = uint.Parse(eqSplit[0].Replace("@", "").Trim());
                    string toAdd = eqSplit[1];
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
        private static void AddStringReference(uint referenceID, string toAdd)
        {
            int startIndex = toAdd.IndexOf('~');
            int lastIndex = toAdd.LastIndexOf('~');
            string substring = toAdd.Substring(startIndex, lastIndex - startIndex);
            if(substring.Contains('[') && substring.Contains(']'))
            {
                int startBracket = substring.IndexOf('[');
                int endBracket = substring.LastIndexOf(']');
                toAdd = toAdd.Replace(substring.Substring(startBracket, (endBracket - startBracket) + 1), "").Trim();
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
            }
            _tlkReferences.Add(referenceID, toAdd);
        }
        public static uint ConvertToReference(string toConvert)
        {
            uint indexToReturn = _firstIndex + (uint)_stringList.Count;
            _stringList.Add(indexToReturn, toConvert);
            return indexToReturn;
        }
        public static string GetString(uint reference)
        {
            if (_tlkReferences.ContainsKey(reference))
            {
                return _tlkReferences[reference];
            }
            return "";
        }
        public static byte[] AddUsedReference(uint reference)
        {
            if(reference == UInt32.MaxValue || !_tlkReferences.ContainsKey(reference))
            {
                return new byte[] { 0xff, 0xff, 0xff, 0xff };
            }
            string referenceText = _tlkReferences[reference];
            uint newReferenceID = (uint)_usedReferences.Count;
            _usedReferences.Add(newReferenceID, referenceText);
            return BitConverter.GetBytes(newReferenceID);
        }
        public static void WriteTRA(string postConversionDirectory)
        {
            string output = "";
            foreach(uint key in _stringList.Keys)
            {
                output += "@" + key + " = " + _stringList[key] + Environment.NewLine;
            }
            File.WriteAllText(postConversionDirectory + "generated.tra", output);
        }
    }
}
