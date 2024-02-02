using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class MasterTRA
    {
        private Dictionary<uint, string> _tlkReferences;
        private Dictionary<uint, string> _usedReferences;
        public MasterTRA(string tlkFile) 
        { 
            _tlkReferences = new Dictionary<uint, string>();
            _usedReferences = new Dictionary<uint, string>();
            string[] lines = File.ReadAllLines(tlkFile);

            for(uint i =0; i < lines.Length; i++)
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
        public string GetString(uint reference)
        {
            if (_tlkReferences.ContainsKey(reference))
            {
                return _tlkReferences[reference];
            }
            return "";
        }
        public byte[] AddUsedReference(uint reference)
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
    }
}
