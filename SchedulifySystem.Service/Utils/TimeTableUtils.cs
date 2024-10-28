using SchedulifySystem.Service.BusinessModels.ClassPeriodBusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace SchedulifySystem.Service.Utils
{
    public static class TimeTableUtils
    {
        private static readonly Random rand = new();
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            T[] elements = source.ToArray();
            for (int i = elements.Length - 1; i >= 0; i--)
            {
                int swapIndex = rand.Next(i + 1);
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }
        }

        public static void Swap(ClassPeriodScheduleModel a, ClassPeriodScheduleModel b)
        {
            (a.StartAt, b.StartAt) = (b.StartAt, a.StartAt);
        }
    }
}
