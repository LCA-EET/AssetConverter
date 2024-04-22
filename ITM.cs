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
        private int _effectStartOffset;
        public ITM(string preConversionPath, string postConversionPath, IEResRef resRef) : base(preConversionPath, postConversionPath, resRef)
        {
            _stringReferences = new StringReferenceTable();
            _stringReferences.AddOffsetEntry(StringIdentifier.NAME1, 0x08);
            _stringReferences.AddOffsetEntry(StringIdentifier.NAME2, 0x0C);
            _stringReferences.AddOffsetEntry(StringIdentifier.UNIDENTIFIED_DESC, 0x50);
            _stringReferences.AddOffsetEntry(StringIdentifier.DESC, 0x54);
            _stringReferences.ResolveReferences(_contents);
            _effectStartOffset = BitConverter.ToInt32(_contents, 0x6A);
            ReplaceBAMReferences();
            ReplaceAbilities();
            ReplaceEffects();
        }
        private void ReplaceEffects()
        {
            int effectsOffset = BitConverter.ToInt32(_contents, 0x6A);
            int numGlobalEffects = BitConverter.ToInt16(_contents, 0x70);

            for (int i = 0; i < numGlobalEffects; i++)
            {
                ProcessEffect(effectsOffset);
                effectsOffset += 0x30;
            }
        }
        private void ProcessEffect(int offset)
        {
            int effectType = BitConverter.ToInt16(_contents, offset);
            switch (effectType)
            {
                case 146: // cast spell
                    ReplaceReference(offset + 20, "spl");
                    break;
                case 177: // Use EFF File
                    ReplaceReference(offset + 20, "eff");
                    break;
            }
        }
        private void ReplaceAbilities()
        {
            int abilitiesOffset = BitConverter.ToInt32(_contents, 0x64);
            int numAbilities = BitConverter.ToInt16(_contents, 0x68);

            for (int i = 0; i < numAbilities; i++)
            {
                ReplaceReference(abilitiesOffset + 4, "bam");
                int numEffects = BitConverter.ToInt16(_contents, abilitiesOffset + 30);
                int firstEffectIndex = BitConverter.ToInt16(_contents, abilitiesOffset + 32);
                int localOffset = _effectStartOffset + (firstEffectIndex * 0x30);
                for (int j = 0; j < numEffects; j++)
                {
                    ProcessEffect(localOffset);
                    localOffset += 0x30;
                }
                abilitiesOffset += 0x38;
            }
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
