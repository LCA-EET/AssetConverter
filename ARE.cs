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
        //private List<Trigger> _triggers;
        private Dictionary<int, int> _songTable;
        public ARE(string preConversionPath, string postConversionPath, IEResRef resRef) : base(preConversionPath, postConversionPath, resRef)
        {
            _stringReferences = new StringReferenceTable();
            //_triggers = new List<Trigger>();
            _songTable = new Dictionary<int, int>();
            ReplaceActors();
            ReplaceAREComponents();
            ReplaceAmbients();
            ReplaceDoorKeys();
            ReplaceTriggers();
            ReplaceItems();
            ReplaceMapNotes();
            ReplaceContainerKeys();
            ReplaceSpawnPoints();
            if (Program.paramFile.IncludeAreaScripts)
            {
                ReplaceReference(0x94, "baf", _owningReference.ReferenceBytes); // Area Script
            }
            else
            {
                string newPrefix = Program.paramFile.Prefix + "s" + _owningReference.NewReferenceID.Substring(3);
                List<byte> asReferenceBytes = new List<byte>(Encoding.Latin1.GetBytes(newPrefix));
                while(asReferenceBytes.Count < 8) 
                {
                    asReferenceBytes.Add(0x00);
                }
                ReplaceReference(0x94, "baf", asReferenceBytes.ToArray(), true); // Area Script
                if(!Directory.Exists(Program.paramFile.PostconversionDirectory + "baf"))
                {
                    Directory.CreateDirectory(Program.paramFile.PostconversionDirectory + "baf");
                }
                File.WriteAllBytes(Program.paramFile.PostconversionDirectory + "baf\\" + newPrefix + ".baf", new byte[] {  });
            }
            
            ReplaceReference(0x08, "wed", _owningReference.ReferenceBytes); // WED
            ReplaceAnimations();
            GenerateSongList();
        }
        private void ReplaceSpawnPoints()
        {
            int spawnPointOffset = BitConverter.ToInt32(_contents, 0x60);
            int numPoints = BitConverter.ToInt32(_contents, 0x64);

            for(int i = 0; i < numPoints; i++)
            {
                int numCreatures = BitConverter.ToInt16(_contents, spawnPointOffset + 0x74);
                for(int j = 0; j < numCreatures; j++)
                {
                    ReplaceReference(spawnPointOffset + 0x24 + (j * 0x08), "cre");
                }
                spawnPointOffset += 0xC8;
            }
        }
        private void ReplaceMapNotes()
        {
            int noteOffset = BitConverter.ToInt32(_contents, 0xC4);
            int numNotes = BitConverter.ToInt32(_contents, 0xC8);
            int textRef = 0;
            for(int i = 0; i < numNotes; i++)
            {
                textRef = BitConverter.ToInt32(_contents, noteOffset + 4);
                if (textRef > 0)
                {
                    _stringReferences.AddLong(noteOffset + 4, textRef);
                }
                noteOffset += 0x34;
            }
        }
        private void GenerateSongList()
        {
            int songsOffset = BitConverter.ToInt32(_contents, 0xbc);
            int numSongs = 10;
            for(int i = 0; i < numSongs; i++)
            {
                int songID = BitConverter.ToInt32(_contents, songsOffset);
                if (MusicTable.AddSong(songID))
                {
                    _songTable.Add(songsOffset, songID);
                }
                else
                {
                    _contents[songsOffset] = 0x00;
                    _contents[songsOffset + 1] = 0x00;
                    _contents[songsOffset + 2] = 0x00;
                    _contents[songsOffset + 3] = 0x00;
                }
                songsOffset += 4;
            }
        }
        private void ReplaceAnimations()
        {
            int numAnimations = BitConverter.ToInt32(_contents, 0xAC);
            int offset = BitConverter.ToInt32(_contents, 0xB0);
            for(int i = 0; i < numAnimations; i++)
            {
                ReplaceReference(offset + 0x28, "bam");
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
                offset += 0x14;
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
                if (Program.paramFile.IncludeDialogs)
                {
                    ReplaceReference((intermediateOffset), "dlg");
                }
                else
                {
                    WriteNullBytes(0x2cc, 8);
                }
               
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
                        toReturn += "\tWRITE_LONG " + songOffset + " %" + Program.paramFile.Prefix + MusicTable.GetUpdatedReference(songID) + "%" + Environment.NewLine; 
                    }
                }
                return toReturn;
            }
        }
        public override string ToTP2String()
        {
            string toReturn = "ACTION_IF NOT(FILE_EXISTS_IN_GAME ~" + _owningReference.NewReferenceID + "." + _owningReference.ResourceType + "~) BEGIN" + Environment.NewLine;
            ///toReturn += "\t//PRINT ~Area already exists: " + _owningReference.NewReferenceID + ". Skipping...~" + Environment.NewLine;
            //toReturn += "END ELSE BEGIN" + Environment.NewLine;
            toReturn += "\t" + base.ToTP2String();
            toReturn += _stringReferences.TP2String();
            toReturn += SongList;
            toReturn += "END" + Environment.NewLine;
            return toReturn;
        }
    }
}
