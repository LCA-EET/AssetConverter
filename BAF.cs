using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class BAF : IEAsset
    {
        private string _text; 
        public BAF(string preConversionPath, string postConversionPath, IEResRef resRef) : base(preConversionPath, postConversionPath, resRef) 
        {
            //_text = File.ReadAllText(preConversionPath);
            
        }
        public void PerformPostProcessing()
        {
            RemovePipes();
        }
        private void RemovePipes()
        {
            _text = File.ReadAllText(PostConversionPath);
            if(_text.Contains("|"))
            {
                string[] lines = File.ReadAllLines(PostConversionPath);
                int ifLine = 0;
                int endLine = 0;
                for(int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("|"))
                    {
                        lines[i] = lines[i].Replace("|", "::");
                        for(int j = i; j >= 0; j--)
                        {
                            if (lines[j] == "IF")
                            {
                                ifLine = j;
                                lines[ifLine] = "/*IF";
                                break;
                            }
                        }

                        for(int j = i; j <lines.Length; j++)
                        {
                            if (lines[j] == "END")
                            {
                                endLine = j;
                                lines[endLine] = "END*/";
                                break;
                            }
                        }
                    }
                }
                File.WriteAllLines(PostConversionPath, lines);
                //_contents = File.ReadAllBytes(PostConversionPath);
            }
        }
        
    }
}
