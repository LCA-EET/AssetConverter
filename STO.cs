using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetConverter
{
    public class STO : IEAsset
    {
        private StringReferenceTable _stringReferences;

        public STO(string preConversionPath, string postConversionPath, IEResRef owningReference) : base(preConversionPath, postConversionPath, owningReference)
        {
            _stringReferences = new StringReferenceTable();
            _stringReferences.AddLong(0x0C, BitConverter.ToInt32(_contents, 0x0C));
            //_stringReferences.ResolveReferences(_contents);
            ReplaceDrinksForSale();
            ReplaceRumors();
            ReplaceItemsForSale();
            
        }
        private void ReplaceDrinksForSale()
        {
            int numDrinksForSale = BitConverter.ToInt32(_contents, 0x50);
            int drinksForSaleOffset = BitConverter.ToInt32(_contents, 0x4C);
            for (int i = 0; i < numDrinksForSale; i++)
            {
                _stringReferences.AddLong(drinksForSaleOffset + 8, BitConverter.ToInt32(_contents, drinksForSaleOffset + 8));
                drinksForSaleOffset += 0x14;
            }
        }
        private void ReplaceItemsForSale()
        {
            int numItemsForSale = BitConverter.ToInt32(_contents, 0x38);
            int itemForSaleOffset = BitConverter.ToInt32(_contents, 0x34);
            for(int i = 0; i < numItemsForSale; i++)
            {
                ReplaceReference(itemForSaleOffset, "itm");
                itemForSaleOffset += 0x1C;
            }
        }
        private void ReplaceRumors()
        {
            ReplaceReference(0x44, "dlg"); //drinks
            ReplaceReference(0x54, "dlg"); //donation
        }

        public override string ToTP2String()
        {
            string toReturn = base.ToTP2String();
            toReturn += _stringReferences.TP2String();
            return toReturn;
        }
    }
}
