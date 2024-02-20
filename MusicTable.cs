using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public static class MusicTable
    {
        private static Dictionary<int, int> _musicTable;
        private static int _firstID;

        public static void Initialize(int firstID)
        {
            _firstID = firstID;
            _musicTable = new Dictionary<int, int>();
        }
        public static void AddSong(int id)
        {
            if(id > 0)
            {
                if(!_musicTable.ContainsKey(id))
                {
                    _musicTable.Add(id, _firstID);
                    _firstID++;
                }
            }
        }
        public static bool SongExists(int songID)
        {
            return _musicTable.ContainsKey(songID);
        }

        public static int GetUpdatedReference(int songID)
        {
            return _musicTable[songID];
        }

        public static string TableToString()
        {
            string toReturn = "";
            foreach(int id in _musicTable.Keys)
            {
                toReturn += id + ":" + _musicTable[id] + Environment.NewLine;
            }
            return toReturn;
        }
    }
}
