using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
  
    public class CRE : IEAsset
    {
        private StringReferenceTable _stringReferences;
        public CRE(string preConversionPath, string postConversionPath, IEResRef resRef) : base(preConversionPath, postConversionPath, resRef)
        {
            _stringReferences = new StringReferenceTable();
            _stringReferences.AddOffsetEntry(StringIdentifier.NAME1, 0x08);
            _stringReferences.AddOffsetEntry(StringIdentifier.NAME2, 0x0C);
            _stringReferences.AddOffsetEntry(StringIdentifier.INITIAL_MEETING, 0xA4);
            _stringReferences.AddOffsetEntry(StringIdentifier.MORALE, 0xA8);
            _stringReferences.AddOffsetEntry(StringIdentifier.HAPPY, 0xAC);
            _stringReferences.AddOffsetEntry(StringIdentifier.UNHAPPY_ANNOYED, 0xB0);
            _stringReferences.AddOffsetEntry(StringIdentifier.UNHAPPY_SERIOUS, 0xB4);
            _stringReferences.AddOffsetEntry(StringIdentifier.UNHAPPY_BREAKING_POINT, 0xB8);
            _stringReferences.AddOffsetEntry(StringIdentifier.LEADER, 0xBC);
            _stringReferences.AddOffsetEntry(StringIdentifier.TIRED, 0xC0);
            _stringReferences.AddOffsetEntry(StringIdentifier.BORED, 0xC4);
            _stringReferences.AddOffsetEntry(StringIdentifier.BATTLE_CRY1, 0xC8);
            _stringReferences.AddOffsetEntry(StringIdentifier.BATTLE_CRY2, 0xCC);
            _stringReferences.AddOffsetEntry(StringIdentifier.BATTLE_CRY3, 0xD0);
            _stringReferences.AddOffsetEntry(StringIdentifier.BATTLE_CRY4, 0xD4);
            _stringReferences.AddOffsetEntry(StringIdentifier.BATTLE_CRY5, 0xD8);
            _stringReferences.AddOffsetEntry(StringIdentifier.ATTACK1, 0xDC);
            _stringReferences.AddOffsetEntry(StringIdentifier.ATTACK2, 0xE0);
            _stringReferences.AddOffsetEntry(StringIdentifier.ATTACK3, 0xE4);
            _stringReferences.AddOffsetEntry(StringIdentifier.ATTACK4, 0xE8);
            _stringReferences.AddOffsetEntry(StringIdentifier.DAMAGE, 0xEC);
            _stringReferences.AddOffsetEntry(StringIdentifier.DYING, 0xF0);
            _stringReferences.AddOffsetEntry(StringIdentifier.HURT, 0xF4);
            _stringReferences.AddOffsetEntry(StringIdentifier.AREA_FOREST, 0xF8);
            _stringReferences.AddOffsetEntry(StringIdentifier.AREA_CITY, 0xFC);
            _stringReferences.AddOffsetEntry(StringIdentifier.AREA_DUNGEON, 0x100);
            _stringReferences.AddOffsetEntry(StringIdentifier.AREA_DAY, 0x104);
            _stringReferences.AddOffsetEntry(StringIdentifier.AREA_NIGHT, 0x108);
            _stringReferences.AddOffsetEntry(StringIdentifier.SELECT_COMMON1, 0x10C);
            _stringReferences.AddOffsetEntry(StringIdentifier.SELECT_COMMON2, 0x110);
            _stringReferences.AddOffsetEntry(StringIdentifier.SELECT_COMMON3, 0x114);
            _stringReferences.AddOffsetEntry(StringIdentifier.SELECT_COMMON4, 0x118);
            _stringReferences.AddOffsetEntry(StringIdentifier.SELECT_COMMON5, 0x11C);
            _stringReferences.AddOffsetEntry(StringIdentifier.SELECT_COMMON6, 0x120);
            _stringReferences.AddOffsetEntry(StringIdentifier.SELECT_ACTION1, 0x124);
            _stringReferences.AddOffsetEntry(StringIdentifier.SELECT_ACTION2, 0x128);
            _stringReferences.AddOffsetEntry(StringIdentifier.SELECT_ACTION3, 0x12C);
            _stringReferences.AddOffsetEntry(StringIdentifier.SELECT_ACTION4, 0x130);
            _stringReferences.AddOffsetEntry(StringIdentifier.SELECT_ACTION5, 0x134);
            _stringReferences.AddOffsetEntry(StringIdentifier.SELECT_ACTION6, 0x138);
            _stringReferences.AddOffsetEntry(StringIdentifier.SELECT_ACTION7, 0x13C);
            _stringReferences.AddOffsetEntry(StringIdentifier.INTERACTION1, 0x140);
            _stringReferences.AddOffsetEntry(StringIdentifier.INTERACTION2, 0x144);
            _stringReferences.AddOffsetEntry(StringIdentifier.INTERACTION3, 0x148);
            _stringReferences.AddOffsetEntry(StringIdentifier.INTERACTION4, 0x14C);
            _stringReferences.AddOffsetEntry(StringIdentifier.INTERACTION5, 0x150);
            _stringReferences.AddOffsetEntry(StringIdentifier.INSULT1, 0x154);
            _stringReferences.AddOffsetEntry(StringIdentifier.INSULT2, 0x158);
            _stringReferences.AddOffsetEntry(StringIdentifier.INSULT3, 0x15C);
            _stringReferences.AddOffsetEntry(StringIdentifier.COMPLIMENT1, 0x160);
            _stringReferences.AddOffsetEntry(StringIdentifier.COMPLIMENT2, 0x164);
            _stringReferences.AddOffsetEntry(StringIdentifier.COMPLIMENT3, 0x168);
            _stringReferences.AddOffsetEntry(StringIdentifier.SPECIAL1, 0x16C);
            _stringReferences.AddOffsetEntry(StringIdentifier.SPECIAL2, 0x170);
            _stringReferences.AddOffsetEntry(StringIdentifier.SPECIAL3, 0x174);
            _stringReferences.AddOffsetEntry(StringIdentifier.REACT_TO_DIE_GENERAL, 0x178);
            _stringReferences.AddOffsetEntry(StringIdentifier.REACT_TO_DIE_SPECIFIC, 0x17C);
            _stringReferences.AddOffsetEntry(StringIdentifier.RESPONSE_TO_COMPLIMENT1, 0x180);
            _stringReferences.AddOffsetEntry(StringIdentifier.RESPONSE_TO_COMPLIMENT2, 0x184);
            _stringReferences.AddOffsetEntry(StringIdentifier.RESPONSE_TO_COMPLIMENT3, 0x188);
            _stringReferences.AddOffsetEntry(StringIdentifier.RESPONSE_TO_INSULT1, 0x18C);
            _stringReferences.AddOffsetEntry(StringIdentifier.RESPONSE_TO_INSULT2, 0x190);
            _stringReferences.AddOffsetEntry(StringIdentifier.RESPONSE_TO_INSULT3, 0x194);
            _stringReferences.AddOffsetEntry(StringIdentifier.DIALOG_HOSTILE, 0x198);
            _stringReferences.AddOffsetEntry(StringIdentifier.DIALOG_DEFAULT, 0x19C);
            _stringReferences.AddOffsetEntry(StringIdentifier.SELECT_RARE1, 0x1A0);
            _stringReferences.AddOffsetEntry(StringIdentifier.SELECT_RARE2, 0x1A4);
            _stringReferences.AddOffsetEntry(StringIdentifier.CRITICAL_HIT, 0x1A8);
            _stringReferences.AddOffsetEntry(StringIdentifier.CRITICAL_MISS, 0x1AC);
            _stringReferences.AddOffsetEntry(StringIdentifier.TARGET_IMMUNE, 0x1B0);
            _stringReferences.AddOffsetEntry(StringIdentifier.INVENTORY_FULL, 0x1B4);
            _stringReferences.AddOffsetEntry(StringIdentifier.PICKED_POCKET, 0x1B8);
            _stringReferences.AddOffsetEntry(StringIdentifier.HIDDEN_IN_SHADOWS, 0x1BC);
            _stringReferences.AddOffsetEntry(StringIdentifier.SPELL_DISRUPTED, 0x1C0);
            _stringReferences.AddOffsetEntry(StringIdentifier.SET_A_TRAP, 0x1C4);
            _stringReferences.AddOffsetEntry(StringIdentifier.EXISTANCE4, 0x1C8);
            _stringReferences.AddOffsetEntry(StringIdentifier.BIO, 0x1CC);
            _stringReferences.AddOffsetEntry(StringIdentifier.BG2EE_SELECT_RARE1, 0x1D0);
            _stringReferences.AddOffsetEntry(StringIdentifier.BG2EE_SELECT_RARE2, 0x1D4);
            _stringReferences.AddOffsetEntry(StringIdentifier.BG2EE_SELECT_RARE3, 0x1D8);
            _stringReferences.AddOffsetEntry(StringIdentifier.BG2EE_SELECT_RARE4, 0x1DC);
            _stringReferences.AddOffsetEntry(StringIdentifier.BGEE_ACTION4, 0x1E0);
            _stringReferences.AddOffsetEntry(StringIdentifier.BGEE_ACTION5, 0x1E4);
            _stringReferences.AddOffsetEntry(StringIdentifier.BGEE_ACTION6, 0x1E8);
            _stringReferences.AddOffsetEntry(StringIdentifier.BGEE_ACTION7, 0x1EC);
            _stringReferences.AddOffsetEntry(StringIdentifier.IWDEE_MORALE2, 0x1F0);
            _stringReferences.AddOffsetEntry(StringIdentifier.IWDEE_LEADER2, 0x1F4);
            _stringReferences.AddOffsetEntry(StringIdentifier.IWDEE_TIRED2, 0x1F8);
            _stringReferences.AddOffsetEntry(StringIdentifier.IWDEE_BORED2, 0x1FC);
            _stringReferences.AddOffsetEntry(StringIdentifier.IWDEE_HURT2, 0x200);
            _stringReferences.AddOffsetEntry(StringIdentifier.IWDEE_SELECT_COMMON7, 0x204);
            _stringReferences.AddOffsetEntry(StringIdentifier.IWDEE_DAMAGE2, 0x208);
            _stringReferences.AddOffsetEntry(StringIdentifier.IWDEE_DAMAGE3, 0x20C);
            _stringReferences.AddOffsetEntry(StringIdentifier.IWDEE_DYING2, 0x210);
            _stringReferences.AddOffsetEntry(StringIdentifier.IWDEE_REACT_TO_DIE_GENERAL2, 0x214);
            _stringReferences.ResolveReferences(_contents);
            ReplaceScriptReferences();
            ReplaceItemReferences();
            ReplaceScriptName();
            ReplaceReference(0x34, "bmp");
            ReplaceReference(0x3C, "bmp");
            //_contents[0x274] = 0x00;
            ReplaceDLG();
        }
        private void ReplaceScriptName()
        {
            int scriptNameOffset = 0x280;
            byte[] newReferenceID = _owningReference.ReferenceBytes;
            int refIdx = 0;
            for(int i = scriptNameOffset; i < scriptNameOffset + newReferenceID.Length; i++)
            {
                _contents[i] = newReferenceID[refIdx];
                refIdx++;
            }
        }
        private void ReplaceItemReferences()
        {
            int itemOffset = BitConverter.ToInt32(_contents, 0x2BC);
            int numItems = BitConverter.ToInt32(_contents, 0x2C0);
            for(int i = 0; i < numItems; i++)
            {
                ReplaceReference(itemOffset, "itm");
                itemOffset += 0x14;
            }
        }
        private void ReplaceScriptReferences()
        {
            int[] offsets = { 0x248, 0x250, 0x258, 0x260, 0x268 };
            for (int i = 0; i < offsets.Length; i++)
            {
                ReplaceReference(offsets[i], "baf");
            }
        }
        private void ReplaceDLG()
        {
            if (Program.paramFile.IncludeDialogs)
            {
                ReplaceReference(0x2cc, "dlg");
            }
            else
            {
                WriteNullBytes(0x2cc, 8);
            }
            
        }

        public override string ToTP2String()
        {
            string toReturn =  base.ToTP2String();
            toReturn += _stringReferences.TP2String() + Environment.NewLine;
            return toReturn;
        }
    }
}
