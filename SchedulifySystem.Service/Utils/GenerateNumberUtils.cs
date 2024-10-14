using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Utils
{
    public class GenerateNumberUtils
    {
        public static int GenerateDigitNumber()
        {
            Random random = new();
            int number = random.Next(100000, 1000000);
            return number;
        }
    }
}
