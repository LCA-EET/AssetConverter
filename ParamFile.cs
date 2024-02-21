using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public  class ParamFile
    {
        public string QueuePath { get; set; }
        public string PreconversionDirectory { get; set; }
        public string PostconversionDirectory { get; set; }
        public string Prefix { get; set; }
        public string WeiduDirectory { get; set; }
        public string WeiduPath { get; set; }
        public string ModFolder { get; set; }

        public int FirstTRAIndex { get; set; }

        public int FirstID { get; set; }
        public int FirstWAVID { get; set; }
        public bool IncludeWAVs { get; set; }
        public int MusicIndex { get; set; }

        public string SongListPath { get; set; }
        public string MusicDirectory { get; set; }

        public bool IncludeAreaScripts { get; set; }
        public ParamFile(string filePath) 
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Could not locate file: " + filePath + ". Exiting.");
                Environment.Exit(0);
                return;
            }
            string[] lines = File.ReadAllLines(filePath);
            try
            {
                for(int i = 0; i < 3; i++)
                {
                    if (!Directory.Exists(lines[i]))
                    {
                        Console.WriteLine("Could not locate directory: " + lines[i] + ". Exiting.");
                        Environment.Exit(0);
                        return;
                    }
                    if (!lines[i].EndsWith("\\"))
                    {
                        lines[i] += "\\";
                    }
                }
                if (!File.Exists(lines[3]))
                {
                    Console.WriteLine("Could not locate file: " + lines[3] + ". Exiting.");
                }
                PreconversionDirectory = lines[0];
                PostconversionDirectory = lines[1];
                WeiduDirectory = lines[2];
                WeiduPath = WeiduDirectory + "weidu.exe";
                QueuePath = lines[3];
                Prefix = lines[4];
                ModFolder = lines[5];
                FirstTRAIndex = int.Parse(lines[6]);
                FirstID = int.Parse(lines[7]);
                FirstWAVID = int.Parse(lines[8]);
                string includeWAVs = lines[9];
                if(int.Parse(includeWAVs) == 1)
                {
                    IncludeWAVs = true;
                }
                else
                {
                    IncludeWAVs = false;
                }
                MusicIndex = int.Parse(lines[10]);
                SongListPath = lines[11];
                MusicDirectory = lines[12];
                IncludeAreaScripts = int.Parse(lines[13]) == 1 ? true : false;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(0);
            }
        }
    }
}
