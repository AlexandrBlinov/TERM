using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace YstProject.Services
{
    public class StringAsNumberComparer : IComparer<string>
    {
        public StringAsNumberComparer()
        {

        }
        public int Compare(string x, string y)
        {
            decimal result1, result2;

            if (Decimal.TryParse(x, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result1) && Decimal.TryParse(y, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result2))
            {
                return Decimal.Compare(result1, result2);

            }
            return String.Compare(x, y);

        }
    }
}
