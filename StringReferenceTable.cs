using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public static class StringIdentifier
    {
        //ITM
        public static string NAME1 = "NAME1";
        public static string NAME2 = "NAME2";
        public static string UNIDENTIFIED_DESC = "UNIDENTIFIED_DESC";
        public static string DESC = "DESC";

        //CRE
        
        public static string INITIAL_MEETING = "INITIAL_MEETING";
        public static string MORALE = "MORALE";
        public static string HAPPY = "HAPPY";
        public static string UNHAPPY_ANNOYED = "UNHAPPY_ANNOYED";
        public static string UNHAPPY_SERIOUS = "UNHAPPY_SERIOUS";
        public static string UNHAPPY_BREAKING_POINT = "UNHAPPY_BREAKING_POINT";
        public static string LEADER = "LEADER";
        public static string TIRED = "TIRED";
        public static string BORED = "BORED";
        public static string BATTLE_CRY1 = "BATTLE_CRY1";
        public static string BATTLE_CRY2 = "BATTLE_CRY2";
        public static string BATTLE_CRY3 = "BATTLE_CRY3";
        public static string BATTLE_CRY4 = "BATTLE_CRY4";
        public static string BATTLE_CRY5 = "BATTLE_CRY5";
        public static string ATTACK1 = "ATTACK1";
        public static string ATTACK2 = "ATTACK2";
        public static string ATTACK3 = "ATTACK3";
        public static string ATTACK4 = "ATTACK4";
        public static string DAMAGE = "DAMAGE";
        public static string DYING = "DYING";
        public static string HURT = "HURT";
        public static string AREA_FOREST = "AREA_FOREST";
        public static string AREA_CITY = "AREA_CITY";
        public static string AREA_DUNGEON = "AREA_DUNGEON";
        public static string AREA_DAY = "AREA_DAY";
        public static string AREA_NIGHT = "AREA_NIGHT";
        public static string SELECT_COMMON1 = "SELECT_COMMON1";
        public static string SELECT_COMMON2 = "SELECT_COMMON2";
        public static string SELECT_COMMON3 = "SELECT_COMMON3";
        public static string SELECT_COMMON4 = "SELECT_COMMON4";
        public static string SELECT_COMMON5 = "SELECT_COMMON5";
        public static string SELECT_COMMON6 = "SELECT_COMMON6";
        public static string SELECT_ACTION1 = "SELECT_ACTION1";
        public static string SELECT_ACTION2 = "SELECT_ACTION2";
        public static string SELECT_ACTION3 = "SELECT_ACTION3";
        public static string SELECT_ACTION4 = "SELECT_ACTION4";
        public static string SELECT_ACTION5 = "SELECT_ACTION5";
        public static string SELECT_ACTION6 = "SELECT_ACTION6";
        public static string SELECT_ACTION7 = "SELECT_ACTION7";
        public static string INTERACTION1 = "INTERACTION1";
        public static string INTERACTION2 = "INTERACTION2";
        public static string INTERACTION3 = "INTERACTION3";
        public static string INTERACTION4 = "INTERACTION4";
        public static string INTERACTION5 = "INTERACTION5";
        public static string INSULT1 = "INSULT1";
        public static string INSULT2 = "INSULT2";
        public static string INSULT3 = "INSULT3";
        public static string COMPLIMENT1 = "COMPLIMENT1";
        public static string COMPLIMENT2 = "COMPLIMENT2";
        public static string COMPLIMENT3 = "COMPLIMENT3";
        public static string SPECIAL1 = "SPECIAL1";
        public static string SPECIAL2 = "SPECIAL2";
        public static string SPECIAL3 = "SPECIAL3";
        public static string REACT_TO_DIE_GENERAL = "REACT_TO_DIE_GENERAL";
        public static string REACT_TO_DIE_SPECIFIC = "REACT_TO_DIE_SPECIFIC";
        public static string RESPONSE_TO_COMPLIMENT1 = "RESPONSE_TO_COMPLIMENT1";
        public static string RESPONSE_TO_COMPLIMENT2 = "RESPONSE_TO_COMPLIMENT2";
        public static string RESPONSE_TO_COMPLIMENT3 = "RESPONSE_TO_COMPLIMENT3";
        public static string RESPONSE_TO_INSULT1 = "RESPONSE_TO_INSULT1";
        public static string RESPONSE_TO_INSULT2 = "RESPONSE_TO_INSULT2";
        public static string RESPONSE_TO_INSULT3 = "RESPONSE_TO_INSULT3";
        public static string DIALOG_HOSTILE = "DIALOG_HOSTILE";
        public static string DIALOG_DEFAULT = "DIALOG_DEFAULT";
        public static string SELECT_RARE1 = "SELECT_RARE1";
        public static string SELECT_RARE2 = "SELECT_RARE2";
        public static string CRITICAL_HIT = "CRITICAL_HIT";
        public static string CRITICAL_MISS = "CRITICAL_MISS";
        public static string TARGET_IMMUNE = "TARGET_IMMUNE";
        public static string INVENTORY_FULL = "INVENTORY_FULL";
        public static string PICKED_POCKET = "PICKED_POCKET";
        public static string HIDDEN_IN_SHADOWS = "HIDDEN_IN_SHADOWS";
        public static string SPELL_DISRUPTED = "SPELL_DISRUPTED";
        public static string SET_A_TRAP = "SET_A_TRAP";
        public static string EXISTANCE4 = "EXISTANCE4";
        public static string BIO = "BIO";
        public static string BG2EE_SELECT_RARE1 = "BG2EE_SELECT_RARE1";
        public static string BG2EE_SELECT_RARE2 = "BG2EE_SELECT_RARE2";
        public static string BG2EE_SELECT_RARE3 = "BG2EE_SELECT_RARE3";
        public static string BG2EE_SELECT_RARE4 = "BG2EE_SELECT_RARE4";
        public static string BGEE_ACTION4 = "BGEE_ACTION4";
        public static string BGEE_ACTION5 = "BGEE_ACTION5";
        public static string BGEE_ACTION6 = "BGEE_ACTION6";
        public static string BGEE_ACTION7 = "BGEE_ACTION7";
        public static string IWDEE_MORALE2 = "IWDEE_MORALE2";
        public static string IWDEE_LEADER2 = "IWDEE_LEADER2";
        public static string IWDEE_TIRED2 = "IWDEE_TIRED2";
        public static string IWDEE_BORED2 = "IWDEE_BORED2";
        public static string IWDEE_HURT2 = "IWDEE_HURT2";
        public static string IWDEE_SELECT_COMMON7 = "IWDEE_SELECT_COMMON7";
        public static string IWDEE_DAMAGE2 = "IWDEE_DAMAGE2";
        public static string IWDEE_DAMAGE3 = "IWDEE_DAMAGE3";
        public static string IWDEE_DYING2 = "IWDEE_DYING2";
        public static string IWDEE_REACT_TO_DIE_GENERAL2 = "IWDEE_REACT_TO_DIE_GENERAL2";
    }
    public class StringReferenceTable 
    {
        private Dictionary<string, int> _offsetTable;
        private Dictionary<string, string> _resolvedReferences;    
        public StringReferenceTable() 
        { 
            _offsetTable = new Dictionary<string, int>();
            _resolvedReferences = new Dictionary<string, string>();

        }
        private string DereferenceString(byte[] contents, int offset)
        {
            uint referenceID = BitConverter.ToUInt32(contents, offset);
            return MasterTRA.GetString(referenceID);
        }
        private void AddResolvedReference(string identifier, string text)
        {
            if (!_resolvedReferences.ContainsKey(identifier))
            {
                _resolvedReferences.Add(identifier, text);
            }
            else
            {
                _resolvedReferences[identifier] = text;
            }
        }
        public void AddOffsetEntry(string identifier, int offset)
        {
            if (!_offsetTable.ContainsKey(identifier))
            {
                _offsetTable.Add(identifier, offset);
            }
            else
            {
                _offsetTable[identifier] = offset;
            }
        }
        public void ResolveReferences(byte[] fileContents)
        {
            foreach(string identifier in _offsetTable.Keys)
            {
                int offset = _offsetTable[identifier];
                string dereferenced = DereferenceString(fileContents, offset);
                if(dereferenced != "")
                {
                    AddResolvedReference(identifier, dereferenced);
                }
            }
        }

        public string TP2String()
        {
            string toReturn = "";
            foreach(string key in _resolvedReferences.Keys)
            {
                string text = _resolvedReferences[key]; 
                uint referenceID = MasterTRA.ConvertToReference(text);
                toReturn += "SAY " + key + " @" + referenceID + " /* " + text + " */" +  Environment.NewLine;
            }
            return toReturn;
        }
    }
}
