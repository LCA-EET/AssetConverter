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
        private Dictionary<int, int> _songTable;
        public ARE(string preConversionPath, string postConversionPath, IEResRef resRef) : base(preConversionPath, postConversionPath, resRef)
        {
            _stringReferences = new StringReferenceTable();
            _triggers = new List<Trigger>();
            _songTable = new Dictionary<int, int>();
            ReplaceActors();
            ReplaceAREComponents();
            ReplaceAmbients();
            ReplaceDoorKeys();
            ReplaceTriggers();
            ReplaceItems();
            ReplaceContainerKeys();
            if (Program.paramFile.IncludeAreaScripts)
            {
                ReplaceReference(0x94, "baf", _owningReference.ReferenceBytes); // Area Script
            }
            else
            {
                for(int b = 0; b < 8; b++)
                {
                    _contents[0x94 + b] = 0x00;
                }
            }
            
            ReplaceReference(0x08, "wed", _owningReference.ReferenceBytes); // WED
            ReplaceAnimations();
            GenerateSongList();
        }

        private void GenerateSongList()
        {
            int songsOffset = BitConverter.ToInt32(_contents, 0xbc);
            int numSongs = 10;
            for(int i = 0; i < numSongs; i++)
            {
                int songID = BitConverter.ToInt32(_contents, songsOffset);
                _songTable.Add(songsOffset, songID);
                MusicTable.AddSong(songID);
                songsOffset += 4;
            }
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
                int numSounds = BitConverter.ToInt16(_contents, ambientOffset + 0x80);
                int soundIndex = 0;
                int soundOffset = ambientOffset + 0x30;
                while(soundIndex < numSounds)
                {
                    ReplaceReference(soundOffset, "wav");
                    soundIndex++;
                    soundOffset += 0x08;
                }
                ambientOffset += 0xD4;
            }
        }
        private void ReplaceAREComponents() // Checked OK
        {
            int[] offsets = new int[] {0x18, 0x24, 0x30, 0x3C};
            for(int i = 0; i < offsets.Length; i++)
            {
                ReplaceReference(offsets[i], "are");
            }
        }
        private void ReplaceActors() // Checked OK
        {
            int numActors = BitConverter.ToInt16(_contents, 0x58);
            int actorOffset = BitConverter.ToInt32(_contents, 0x54);
            for (int i = 0; i < numActors; i++)
            {
                int intermediateOffset = actorOffset + 0x48;
                ReplaceReference((intermediateOffset), "dlg");
                intermediateOffset += 0x08;
                ReplaceReference((intermediateOffset), "baf");
                intermediateOffset += 0x08;
                ReplaceReference((intermediateOffset), "baf");
                intermediateOffset += 0x08;
                ReplaceReference((intermediateOffset), "baf");
                intermediateOffset += 0x08;
                ReplaceReference((intermediateOffset), "baf");
                intermediateOffset += 0x08;
                ReplaceReference((intermediateOffset), "baf");
                intermediateOffset += 0x08;
                ReplaceReference((intermediateOffset), "baf");
                intermediateOffset += 0x08;
                ReplaceReference((intermediateOffset), "cre");
                actorOffset += 0x110;
            }
        }
        private string SongList
        {
            get
            {
                string toReturn = "";
                foreach(int songOffset in _songTable.Keys)
                {
                    int songID = _songTable[songOffset];
                    if (MusicTable.SongExists(songID))
                    {
                        toReturn += "\tWRITE_LONG " + songOffset + " %xa" + MusicTable.GetUpdatedReference(songID) + "%" + Environment.NewLine; 
                    }
                }
                return toReturn;
            }
        }
        public override string ToTP2String()
        {
            string toReturn = "ACTION_IF (FILE_EXISTS_IN_GAME ~" + _owningReference.NewReferenceID + "." + _owningReference.ResourceType + "~) BEGIN" + Environment.NewLine;
            toReturn += "\tPRINT ~Area already exists: " + _owningReference.NewReferenceID + ". Skipping...~" + Environment.NewLine;
            toReturn += "END ELSE BEGIN" + Environment.NewLine;
            toReturn += "\t" + base.ToTP2String();
            toReturn += _stringReferences.TP2String();
            toReturn += SongList;
            toReturn += "END" + Environment.NewLine;
            return toReturn;
        }
    }
}
