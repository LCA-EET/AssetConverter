using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class ARE : IEAsset
    {
        private StringReferenceTable _stringReferences;
        private List<Trigger> _triggers;
        public ARE(string preConversionPath, string postConversionPath, IEResRef resRef) : base(preConversionPath, postConversionPath, resRef)
        {
            _stringReferences = new StringReferenceTable();
            _triggers = new List<Trigger>();
            ReplaceActors();
            ReplaceAREComponents();
            ReplaceAmbients();
            ReplaceDoorKeys();
            ReplaceTriggers();
            ReplaceItems();
            ReplaceContainerKeys();
            ReplaceReference(0x94, "baf", _owningReference.ReferenceBytes); // Area Script
            ReplaceReference(0x08, "wed", _owningReference.ReferenceBytes); // WED
            ReplaceAnimations();
        }
        private void ReplaceAnimations()
        {
            int numAnimations = BitConverter.ToInt16(_contents, 0xAC);
            int offset = BitConverter.ToInt32(_contents, 0xB0);
            for(int i = 0; i < numAnimations; i++)
            {
                ReplaceReference(offset + 8, "bam");
                offset += 0x4C;
            }
        }
        private void ReplaceContainerKeys()
        {
            int numContainers = BitConverter.ToInt16(_contents, 0x74);
            int offset = BitConverter.ToInt32(_contents, 0x70);
            for(int i = 0; i < numContainers; i++)
            {
                ReplaceReference(offset + 120, "itm");
                offset += 0xC0;
            }
        }
        
        private void ReplaceItems()
        {
            int numItems = BitConverter.ToInt16(_contents, 0x76);
            int offset = BitConverter.ToInt32(_contents, 0x78);

            for (int i = 0; i < numItems; i++)
            {
                ReplaceReference(offset, "itm");
                offset += 0x0E;
            }
        }

        private void ReplaceTriggers()
        {
            int triggerOffset = BitConverter.ToInt32(_contents, 0x5C);
            int numTriggers = BitConverter.ToInt16(_contents, 0x5A);
            for(int i = 0; i < numTriggers; i++)
            {

                //_stringReferences.AddOffsetEntry()
                int infoTextOffset = triggerOffset + 100;
                int infoTextReference = BitConverter.ToInt32(_contents, infoTextOffset);
                /*
                if (infoTextReference > 0)
                {
                    string triggerName = ResourceManager.ReadString_Latin1(_contents, triggerOffset, 32);
                    string dereferenced = MasterTRA.GetTLKString(infoTextReference);
                    _triggers.Add(new Trigger(triggerName, dereferenced));
                }
                */
                if(infoTextReference > 0)
                {
                    _stringReferences.AddLong(infoTextOffset, infoTextReference);
                }
                ReplaceReference(triggerOffset + 56, "are");
                ReplaceReference(triggerOffset + 116, "itm");
                ReplaceReference(triggerOffset + 124, "baf");
                triggerOffset += 0xC4;
                
            }
        }
        private void ReplaceDoorKeys()
        {
            int doorOffset = BitConverter.ToInt32(_contents, 0xA8);
            int numDoors = BitConverter.ToInt32(_contents, 0xA4);
            for (int i = 0; i < numDoors; i++)
            {
                ReplaceReference((int)(doorOffset + 120), "itm");
                ReplaceReference((int)(doorOffset + 128), "baf");
                ReplaceReference((int)(doorOffset + 184), "dlg");
                doorOffset += 0xC8;
            }
        }
        private void ReplaceAmbients()
        {
            int ambientOffset =  BitConverter.ToInt32(_contents, 0x84);
            int numAmbients = BitConverter.ToInt16(_contents, 0x82);
            for (int i = 0; i < numAmbients; i++)
            {
                for(int j = 0; j < 10; j++) // replace sounds 1 thru 10
                {
                    ReplaceReference((ambientOffset + 48), "wav");
                    ambientOffset += 0x08;
                }
                ambientOffset += 0xD4;
            }
        }
        private void ReplaceAREComponents()
        {
            int[] offsets = new int[] {0x18, 0x24, 0x30, 0x3C};
            for(int i = 0; i < offsets.Length; i++)
            {
                ReplaceReference(offsets[i], "are");
            }
        }
        private void ReplaceActors()
        {
            int numActors = BitConverter.ToInt16(_contents, 0x58);
            int actorOffset = BitConverter.ToInt32(_contents, 0x54);
            for (int i = 0; i < numActors; i++)
            {
                ReplaceReference((actorOffset +128), "cre");
                actorOffset += 0x110;
            }
        }
        public override string ToTP2String()
        {
            string toReturn = base.ToTP2String();
            toReturn += _stringReferences.TP2String();
            /*
            foreach(Trigger trigger in _triggers)
            {
                toReturn += trigger.LPF_ReplaceInfoText();
            }
            */
            return toReturn;
        }
    }
}
