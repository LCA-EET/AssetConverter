using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class DLG : IEAsset
    {
        private string _dPath;
        private string _traPath;
        public DLG(string preConversionPath, string postConversionPath, IEResRef resRef) : base(preConversionPath, postConversionPath, resRef)
        {
            Weidu.DecompileDialog(this);
        }

        public void SetComponentPaths(string dpath, string traPath)
        {
            _dPath = dpath;
            _traPath = traPath;
        }
        
    }
}
