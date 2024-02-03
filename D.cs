using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
namespace AssetConverter
{
    public class D : IEAsset
    {
        public D(string preConversionPath, string postConversionPath, IEResRef resRef) : base(preConversionPath, postConversionPath, resRef)
        {

        }
        public override void SaveAsset(string assetDirectory)
        {
            base.SaveAsset(assetDirectory);
            ReplaceDReferences();
        }
        private void ReplaceDReferences()
        {
            string beginFlag = "BEGIN ~";
            string externFlag = "EXTERN ~";
            bool changeMade = false;
            string[] lineContents = File.ReadAllLines(_postConversionPath);
            bool beginFlagFound = false;
            for (int i = 0; i < lineContents.Length; i++)
            {
                string currentLine = lineContents[i];
                bool lineFlagFound = false;
                if (currentLine.Length > 8)
                {
                    string reference = "";
                    if (!beginFlagFound)
                    {
                        if (currentLine.StartsWith(beginFlag, StringComparison.Ordinal))
                        {
                            reference = currentLine.Split(beginFlag)[1].ToLower();
                            beginFlagFound = true;
                            lineFlagFound = true;
                        }
                    }
                    if (!lineFlagFound)
                    {
                        if (currentLine.Contains(externFlag))
                        {
                            reference = currentLine.Split(externFlag)[1].ToLower();
                            lineFlagFound = true;
                        }
                    }
                    if (lineFlagFound)
                    {
                        reference = reference.Split("~")[0];
                        if (Program.ReferenceTable.ReferenceExists(reference))
                        {
                            string newReferenceID = Program.ReferenceTable.GetReference(reference).ToUpper();
                            lineContents[i] = currentLine.Replace(reference.ToUpper(), newReferenceID);
                            changeMade = true;
                        }
                    }
                }
                
            }
            if (changeMade)
            {
                File.WriteAllLines(_postConversionPath, lineContents);
            }
        }
    }
}
*/