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
        public virtual void SaveAsset()
        {
            File.WriteAllBytes(_postConversionPath, _contents);
        }
        public string ReplaceReference(int offset, string type)
        {
            string reference = DetermineReferenceFromBytes(_contents, offset);
            
            byte[] newReference = null;
            if(reference == "")
            {
                return reference;
            }
            if (ResourceManager.IsResourceLoaded(reference, type))
            {
                newReference = ResourceManager.GetReplacementResource(reference, type);
            }
            else
            {
                newReference = ResourceManager.AddResourceToQueue(reference, type);
            }
            for (int j = 0; j < newReference.Length; j++)
            {
                _contents[offset + j] = newReference[j];
            }
            string toReturn = DetermineReferenceFromBytes(newReference, 0);
            Console.WriteLine("Reference found: " + reference + ". To be replaced with: " + toReturn);
            return toReturn;
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
    }
}
