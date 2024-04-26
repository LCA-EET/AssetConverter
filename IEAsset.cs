using System.Text;

namespace AssetConverter
{
    public class IEAsset
    {
        protected IEResRef _owningReference;
        protected byte[] _contents;
        protected string _preConversionPath;
        protected string _postConversionPath;
        //protected int _idIndex;
        public IEAsset(string preConversionPath, string postConversionPath, IEResRef owningReference)
        {
            _owningReference = owningReference;
            _preConversionPath = preConversionPath;
            _postConversionPath = postConversionPath;
            _contents = File.ReadAllBytes(_preConversionPath);
            
            //_idIndex = 0;
        }
        public IEResRef OwningReference
        {
            get
            {
                return _owningReference;
            }
        }
        public string PreConversionPath
        {
            get
            {
                return _preConversionPath;
            }
        }
        public string PostConversionPath
        {
            get
            {
                return _postConversionPath;
            }
        }
        public virtual void SaveAsset()
        {
            File.WriteAllBytes(_postConversionPath, _contents);
        }
        public string ReplaceReference(int offset, string type, byte[] newResourceID)
        {
            return ReplaceReference(offset, type, newResourceID, false);
        }
        public string ReplaceReference(int offset, string type, byte[] newResourceID, bool skipLoad)
        {
            string reference = DetermineReferenceFromBytes(_contents, offset);
            if (type != "baf" && DAL.AssetExistsInDestination(reference, type))
            {
                return reference;
            }
            byte[] newReference = null;
            if (reference == "" || reference == "None")
            {
                return reference;
            }
            newReference = ResourceManager.AddResourceToQueue(reference, type, newResourceID, skipLoad);
           
            for (int j = 0; j < newReference.Length; j++)
            {
                _contents[offset + j] = newReference[j];
            }
            string toReturn = DetermineReferenceFromBytes(newReference, 0);
            //Console.WriteLine("Reference found: " + reference + ". To be replaced with: " + toReturn);
            return toReturn;
        }
        public string ReplaceReference(int offset, string type)
        {
            return ReplaceReference(offset, type, null);
        }
        public string DetermineReferenceFromBytes(byte[] bytes, int index)
        {
            //references are 8 bytes long
            string toReturn = Encoding.Latin1.GetString(bytes, index, 8);
            int charactersToTrim = 0;
            for (int i = 7; i >= 0; i--)
            {
                if (bytes[index + i] == 0x00)
                {
                    charactersToTrim++;
                }
                else
                {
                    break;
                }
            }
            return toReturn.Substring(0, 8 - charactersToTrim);
        }
        public void WriteNullBytes(int offset, int length)
        {
            for(int i = 0; i < length; i++)
            {
                _contents[offset + i] = 0x00;
            }
        }
        public virtual string ToTP2String()
        {
            string toReturn = "COPY ~" + Program.paramFile.ModFolder + _owningReference.ResourceType + "\\" + _owningReference.NewReferenceID + "." + _owningReference.ResourceType + "~ ~override~" + Environment.NewLine ;
            return toReturn;
        }
    }
}
