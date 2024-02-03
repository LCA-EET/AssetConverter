using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
/*
namespace AssetConverter
{
    public static class ReferenceUpdater
    {
        public static byte[] UpdateComponentReferences(byte[] inBytes)
        {
            List<byte> toReturn = inBytes.ToList();
            for (int i = 0; i < toReturn.Count; i++)
            {
                if (IsCharacterByte(toReturn[i]))
                {
                    if (i + 7 <= (toReturn.Count - 1))
                    {
                        List<byte> nullRemoved = toReturn.GetRange(i, 8);
                        for (int j = nullRemoved.Count - 1; j >= 0; j--)
                        {
                            if (nullRemoved[j] == 0)
                            {
                                nullRemoved.RemoveAt(j);
                            }
                            else
                            {
                                break;
                            }
                        }
                        string toCheck = Encoding.Latin1.GetString(nullRemoved.ToArray()).ToLower().Trim();
                        if (Program.ReferenceTable.ReferenceExists(toCheck))
                        {
                            string replacement = Program.ReferenceTable.GetReference(toCheck);
                            //Log.WriteToLog("... Replaced " + toCheck + " with " + replacement + "\n");//
                            toReturn.RemoveRange(i, 8);
                            toReturn.InsertRange(i, Encoding.Latin1.GetBytes(replacement.ToUpper()).Concat(new byte[] { 0, 0 }));
                            i += 7;
                        }
                    }
                }
            }
            return toReturn.ToArray();
        }

        private static bool IsCharacterByte(byte toCheck)
        {
            //Latin1 Encoding is used
            if (toCheck >= 30 && toCheck <= 39)
            {
                //is a digit
                return true;
            }
            else if (toCheck >= 65 && toCheck <= 90)
            {
                //is a capital letter
                return true;
            }
            else if (toCheck >= 97 && toCheck <= 122)
            {
                //is a capital letter
                return true;
            }
            else if (toCheck == (byte)95)
            {
                //is an underscore
                return true;
            }
            else if (toCheck == (byte)35)
            {
                return true;
            }
            return false;
        }
    }
}
*/