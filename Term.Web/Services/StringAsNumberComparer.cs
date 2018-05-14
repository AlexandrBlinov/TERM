using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Term.DAL;

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

    /// <summary>
    /// Класс для сортировки по производителю
    /// </summary>
    class DiskComparerByProducer : IComparer<int>
    {
        private static readonly IEnumerable<int> producersForgedWheels = Defaults.ProducersForgedWheels.Select(p => p.ProducerId);

        public int Compare(int x, int y)
        {
            if (producersForgedWheels.Any(p => p == x) && !producersForgedWheels.Any(p => p == y)) return -1;
            if (!producersForgedWheels.Any(p => p == x) && producersForgedWheels.Any(p => p == y)) return 1;
            return 0;
        }
    }

    /// <summary>
    /// Класс для сортировки по кованым дискам
    /// </summary>
    class DiskComparerByForged : IComparer<WheelType>
    {
        public int Compare(WheelType x, WheelType y)
        {
            if (x == WheelType.Forged && y != WheelType.Forged) return -1;
            if (x != WheelType.Forged && y == WheelType.Forged) return 1;
            return 0;
        }
    }


}
