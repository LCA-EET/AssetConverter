using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class MasterTRA
    {
        private Dictionary<int, string> _tlkReferences;
        private Dictionary<int, string> _usedReferences;
        public MasterTRA(string tlkFile) 
        { 
            _tlkReferences = new Dictionary<int, string>();
            _usedReferences = new Dictionary<int, string>();
            string[] lines = File.ReadAllLines(tlkFile);

            for(int i =0; i < lines.Length; i++)
            {
                string toAdd = "";
                string[] split = lines[i].Split("=");
                for(int j = 1; j < split.Length; j++)
                {
                    if(j > 1)
                    {
                        toAdd += " ";
                    }
                    toAdd += split[j];
                }
                _tlkReferences.Add(i, toAdd);
            }
        
        }

        public byte[] AddUsedReference(int reference)
        {
            string referenceText = _tlkReferences[reference];
            int newReferenceID = _usedReferences.Count;
            _usedReferences.Add(newReferenceID, referenceText);
            return BitConverter.GetBytes(newReferenceID);
        }
    }
}
