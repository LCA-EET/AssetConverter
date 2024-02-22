using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public static class MusicTable
    {
        private static Dictionary<int, string> _preMusicTable;
        private static Dictionary<int, int> _musicTable;
        private static int _firstID;
        private static string _songListPath, _musicDirectory;
        private static string _postConversionMusicDirectory;
        private static List<string> _musFiles;
        private static Dictionary<string, List<string>> _acmFiles;
        public static void Initialize(int firstID, string songListPath, string musicDirectory)
        {
            _firstID = firstID;
            _musFiles = new List<string>();
            _acmFiles = new Dictionary<string, List<string>>();
            _musicTable = new Dictionary<int, int>();
            _preMusicTable = new Dictionary<int, string>();
            _songListPath = songListPath;
            _musicDirectory = musicDirectory;
            _postConversionMusicDirectory = Program.paramFile.PostconversionDirectory + @"mus\";
            if(!Directory.Exists(_postConversionMusicDirectory))
            {
                Directory.CreateDirectory(_postConversionMusicDirectory);
            }
            ProcessSongListFile();
        }

        private static void ProcessSongListFile()
        {
            string[] lines = File.ReadAllLines(_songListPath);
            for(int i = 3; i < lines.Length; i++)
            {
                string[] relevantData = new string[3];
                int dataIndex = 0;
                string line = lines[i];
                line = line.Replace('\t', ' ');
                string[] split = line.Split(" ");
                if (split.Length > 0)
                {
                    for(int j = 0;  j < split.Length; j++)
                    {
                        if (split[j].Length > 0)
                        {
                            relevantData[dataIndex] = split[j];
                            dataIndex++;
                        }
                    }
                }
                _preMusicTable.Add(int.Parse(relevantData[0]), relevantData[1]);
            }
        }

        public static void AddSong(int id)
        {
            if(id > 0)
            {
                
                if(!_musicTable.ContainsKey(id) && _preMusicTable.ContainsKey(id))
                {
                    _musicTable.Add(id, _firstID);
                    string oldMusicName = _preMusicTable[id].ToLower();
                    string newMusicID = Program.paramFile.Prefix + _firstID;
                    string musicFilePath = Program.paramFile.MusicDirectory  + oldMusicName + ".mus";
                    //Console.WriteLine("Music file path: " + musicFilePath);
                    //Console.ReadLine();
                    if (File.Exists(musicFilePath))
                    {
                        string newMusicPath = _postConversionMusicDirectory + newMusicID + ".mus";
                        File.Copy(musicFilePath, newMusicPath);
                        string musText = File.ReadAllText(newMusicPath);
                        musText = musText.ToUpper().Replace(oldMusicName.ToUpper(), newMusicID.ToUpper());
                        File.WriteAllText(newMusicPath, musText);
                        _musFiles.Add(newMusicPath);
                    }
                    if (Directory.Exists(Program.paramFile.MusicDirectory + @"\" + oldMusicName))
                    {
                        string[] acmFiles = Directory.GetFiles(Program.paramFile.MusicDirectory + @"\" + oldMusicName);
                        if (!Directory.Exists(_postConversionMusicDirectory + newMusicID))
                        {
                            Directory.CreateDirectory(_postConversionMusicDirectory + newMusicID);
                        }
                        foreach (string acmFile in acmFiles)
                        {
                            string[] split = acmFile.Split("\\");
                            string musicName = split[split.Length - 1];
                            musicName = musicName.ToLower().Replace(oldMusicName, newMusicID);
                            string newACMPath = _postConversionMusicDirectory + newMusicID + @"\" + musicName;
                            Console.WriteLine(newACMPath);
                            File.Copy(acmFile, newACMPath);
                            if (!_acmFiles.ContainsKey(newMusicID))
                            {
                                _acmFiles.Add(newMusicID, new List<string>());
                            }
                            _acmFiles[newMusicID].Add(musicName);
                        }
                    }
                    _firstID++;
                }
            }
        }
        public static int SongCount
        {
            get
            {
                return _musicDirectory.Count();
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

            foreach(int id in _preMusicTable.Keys)
            {
                toReturn += id + "|" + _preMusicTable[id] + Environment.NewLine;
            }
            return toReturn;
        }

        public static string ToTP2String()
        {
            
            string toReturn = "//{ MUSIC" + Environment.NewLine;
            int musicID = 0;
            string musFile = "";
            string musVar = "";
            foreach(int key in _musicTable.Keys)
            {
                musicID = _musicTable[key];
                musVar = Program.paramFile.Prefix + musicID;
                musFile = Program.paramFile.Prefix + musicID + ".mus";
                toReturn += "ADD_MUSIC " + musVar + " ~" + Program.paramFile.ModFolder+ @"mus\" + musFile + "~" + Environment.NewLine;
            }
            foreach (string newMusicID in _acmFiles.Keys)
            {
                List<string> acms = _acmFiles[newMusicID];
                foreach(string acm in acms) 
                {
                    toReturn += "COPY ~" + Program.paramFile.ModFolder + @"mus\" + newMusicID + @"\" + acm + @"~ ~MUSIC\" + newMusicID + @"\" + acm + "~" + Environment.NewLine; 
                }
            }
            toReturn += "//}" + Environment.NewLine;
            return toReturn;
        }
    }
}
