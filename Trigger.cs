using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class Trigger
    {
        private string _triggerName;
        private string _dereferenced;

        public Trigger(string triggerName, string dereferenced)
        {
            _triggerName = triggerName;
            _dereferenced = dereferenced;
        }

        public string LPF_ReplaceInfoText()
        {
            string toReturn = "LPF ALTER_AREA_REGION" + Environment.NewLine;
            toReturn += "\tSTR_VAR" + Environment.NewLine;
            toReturn += "\tregion_name = ~" + _triggerName + "~" + Environment.NewLine;
            toReturn += "\tINT_VAR" + Environment.NewLine;
            toReturn += "\tinfo_point = RESOLVE_STR_REF(@" + MasterTRA.ConvertToReference(_dereferenced) + ")" + Environment.NewLine;
            toReturn += "END" + Environment.NewLine;
            return toReturn;
        }
    }
}
