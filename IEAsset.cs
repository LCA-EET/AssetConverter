namespace AssetConverter
{
    public class IEAsset
    {
        protected string _preConversionPath;
        protected string _postConversionPath;
        protected string _assetType;
        protected string _oldName;
        protected string _newName;
        //protected int _idIndex;

        public IEAsset(string preConversionPath, string assetType)
        {
            _preConversionPath = preConversionPath;
            string[] split = _preConversionPath.Split(".");
            string[] pathSplit = split[0].Split("\\");
            _assetType = assetType;
            _oldName = pathSplit[pathSplit.Length - 1].Split(".")[0].ToLower();
            //_idIndex = 0;
        }
        public void SetNewName(string newName)
        {
            _newName = newName;
        }
        public string AssetType 
        {
            get
            {
               return _assetType;
            }
        }
        public string ReferenceID
        {
            get
            {
                return _newName;
            }
        }
        public int IDIndex
        {
            get
            {
                switch (AssetType)
                {
                    case "wav":
                        return 2;
                    case "itm":
                        return 1;
                    default:
                        return 0;
                }
            }
        }
        public string OldReferenceID
        {
            get
            {
                return _oldName;
            }
        }
        public virtual void AssignReferenceID(string referenceID)
        {
            _newName = referenceID;
        }
        public virtual void SaveAsset(string assetDirectory)
        {
            _postConversionPath = assetDirectory + _newName + "." + _assetType;
            File.Copy(_preConversionPath, _postConversionPath);
        }
    }
}
