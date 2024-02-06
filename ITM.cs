using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class ITM : IEAsset
    {
        private StringReferenceTable _stringReferences;
        public ITM(string preConversionPath, string postConversionPath, IEResRef resRef) : base(preConversionPath, postConversionPath, resRef)
        {
            _stringReferences = new StringReferenceTable();
            _stringReferences.AddOffsetEntry(StringIdentifier.NAME1, 0x08);
            _stringReferences.AddOffsetEntry(StringIdentifier.NAME2, 0x0C);
            _stringReferences.AddOffsetEntry(StringIdentifier.UNIDENTIFIED_DESC, 0x50);
            _stringReferences.AddOffsetEntry(StringIdentifier.DESC, 0x54);
            _stringReferences.ResolveReferences(_contents);
            ReplaceBAMReferences();
        }

        private void ReplaceBAMReferences()
        {
            int[] offsets = new int[] { 0x3A, 0x44, 0x58 };
            for (int i = 0; i < offsets.Length; i++)
            {
                ReplaceReference(offsets[i], "bam");
            }
        }

        public override string ToTP2String()
        {
            string toReturn = base.ToTP2String();
            toReturn += _stringReferences.TP2String();
            return toReturn;
        }
    }
}
