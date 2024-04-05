using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class SPL : IEAsset
    {
        private StringReferenceTable _stringReferences;
        public SPL(string preConversionPath, string postConversionPath, IEResRef resRef) : base(preConversionPath, postConversionPath, resRef)
        {
            _stringReferences = new StringReferenceTable();
            ReplaceStringReference(0x8); // spell name
            ReplaceStringReference(0x50); // spell description
            ReplaceReference(0x10, "wav");
            ReplaceReference(0x3A, "bam");
            ProcessAbilities();
        }
        
        private void ReplaceStringReference(int offset)
        {
            _stringReferences.AddLong(offset, BitConverter.ToInt32(_contents, offset));
        }

        private void ProcessAbilities()
        {
            int abilitiesOffset = BitConverter.ToInt32(_contents, 0x64);
            int numAbilities = BitConverter.ToInt16(_contents, 0x68);

            for(int i = 0; i < numAbilities; i++)
            {
                ReplaceReference(abilitiesOffset + 4, "bam"); // spell icon

                abilitiesOffset += 0x28;
            }
        }

        public override string ToTP2String()
        {
            string toReturn = base.ToTP2String();
            toReturn += _stringReferences.TP2String();
            return base.ToTP2String();
        }
    }
}
