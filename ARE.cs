using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class ARE : IEAsset
    {
        
        public ARE(string preConversionPath, string postConversionPath, IEResRef resRef) : base(preConversionPath, postConversionPath, resRef)
        {
            ReplaceActors();
            ReplaceAREComponents();
            ReplaceAmbients();
            ReplaceDoorKeys();
            ReplaceTriggers();
            ReplaceReference(0x94, "baf"); // Area Script
            ReplaceReference(0x08, "wed");
        }
        private void ReplaceContainerContents()
        {

        }
        private void ReplaceTriggers()
        {
            int triggerOffset = BitConverter.ToInt32(_contents, 0x5C);
            int numTriggers = BitConverter.ToInt16(_contents, 0x5A);
            for(int i = 0; i < numTriggers; i++)
            {
                ReplaceReference(triggerOffset + (i * 0xC4) + 56, "are");
                ReplaceReference(triggerOffset + (i * 0xC4) + 116, "itm");
                ReplaceReference(triggerOffset + (i * 0xC4) + 124, "baf");
            }
        }
        private void ReplaceDoorKeys()
        {
            uint doorOffset = BitConverter.ToUInt32(_contents, 0xA8);
            uint numDoors = BitConverter.ToUInt32(_contents, 0xA4);
            for (int i = 0; i < numDoors; i++)
            {
                ReplaceReference((int)(doorOffset + (i * 0xC8) + 120), "itm");
                ReplaceReference((int)(doorOffset + (i * 0xC8) + 128), "baf");
                ReplaceReference((int)(doorOffset + (i * 0xC8) + 184), "dlg");
            }
        }
        private void ReplaceAmbients()
        {
            uint ambientOffset =  BitConverter.ToUInt32(_contents, 0x84);
            uint numAmbients = BitConverter.ToUInt16(_contents, 0x82);
            for (int i = 0; i < numAmbients; i++)
            {
                for(int j = 0; j < 10; j++) // replace sounds 1 thru 10
                {
                    ReplaceReference((int)(ambientOffset + (i * 0xd4) + (48 + (j*8))), "wav");
                }
            }
        }
        private void ReplaceITMComponents()
        {
            
        }
        private void ReplaceAREComponents()
        {
            int[] offsets = new int[] {0x18, 0x24, 0x30, 0x36};
            for(int i = 0; i < offsets.Length; i++)
            {
                ReplaceReference(offsets[i], "are");
            }
        }
        private void ReplaceActors()
        {
            uint numActors = BitConverter.ToUInt16(_contents, 0x58);
            uint actorOffset = BitConverter.ToUInt32(_contents, 0x54);
            for (int i = 0; i < numActors; i++)
            {
                ReplaceReference((int)(actorOffset + (i * 0x110) + 128), "cre");
            }
        }
    }
}
