using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class TRARef
    {
        private string _oldWavID;
        private string _newWavID;
        private string _text;
        private string _flag;
        private int _oldID;
        private int _newID;
        private bool _wavIncluded;
        public TRARef(int oldID, string text)
        {
            _oldID = oldID;
            _oldWavID = "";
            //_text = text;
            _wavIncluded = false;
            _flag = "~";
            int startIndex, endIndex;
            if (text.Contains("%"))
            {
                int percentIdx = text.IndexOf("%");
                int flagIndex = text.IndexOf("~");
                if(percentIdx < flagIndex)
                {
                    _flag = "%";
                }
            }
            string[] split = text.Split(_flag);
            _text = _flag + split[1] + _flag;
            if(_text.Contains("[") && _text.Contains("]"))
            {
                startIndex = text.IndexOf("[");
                endIndex = text.IndexOf("]");
                if(endIndex > startIndex)
                {
                    string toReplace = _text.Substring(startIndex-1, endIndex - (startIndex - 1));
                    //Console.WriteLine(toReplace);
                    _text = _text.Replace(toReplace, "").Trim();
                }
                

            }
            if(split.Length >= 3)
            {
                if (split[2].Length > 0)
                {
                    string soundSection = split[2];
                    if (soundSection.Contains("[") && soundSection.Contains("]"))
                    {
                        _wavIncluded = true;
                        startIndex = soundSection.IndexOf("[");
                        endIndex = soundSection.IndexOf("]");
                        _oldWavID = soundSection.Substring(startIndex +1 , endIndex - startIndex -1);
                        //Console.WriteLine(_oldWavID);
                    }
                }
            }
            
            
        }
        public void CopyWAV()
        {
            if (_wavIncluded)
            {
                byte[] replacementReference = ResourceManager.TrimTrailingNullBytes(ResourceManager.AddResourceToQueue(_oldWavID.ToLower(), "wav", false));
                _newWavID = Encoding.Latin1.GetString(replacementReference);
            }
        }
        public void ReReference(int newID)
        {
            _newID = newID;
        }
        public string TLKString
        {
            get
            {
                if (_wavIncluded)
                {
                    return _text + "[" + _newWavID + "]";
                }
                else
                {
                    return _text;
                }
            }
        }
    }
}
