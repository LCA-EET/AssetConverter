using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public static class Log
    {
        static int _nextLine = 0;
        static string _output = "";
        public static void WriteToLog(string toWrite)
        {
            _output+= toWrite;
            if(_nextLine > 29)
            {
                _nextLine = 0;
                Console.Clear();
            }
            Console.SetCursorPosition(0, _nextLine);
            if (toWrite.EndsWith("\n"))
            {
                _nextLine++;
            }
            Console.Write(toWrite);
        }
        public static void WriteLineToLog(string toWrite)
        {
            WriteToLog("\n");
            WriteToLog(toWrite);
        }
    }
}
