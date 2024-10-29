using SchedulifySystem.Service.BusinessModels.ClassPeriodBusinessModels;
using SchedulifySystem.Service.BusinessModels.ScheduleBusinessMoldes;
using SchedulifySystem.Service.BusinessModels.StudentClassBusinessModels;
using SchedulifySystem.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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

        public static void ToCsv(this TimetableIndividual src)
        {
            var path = "D:\\WorkPlace\\CSV\\Timetable.csv";
            var errorPath = "D:\\WorkPlace\\CSV\\TimetableError.txt";

            //var path = "D:\\Workspace\\dotnet-asp\\fix\\10-be\\Timetable.csv";
            //var errorPath = "D:\\Workspace\\dotnet-asp\\fix\\10-be\\TimetableError.txt";

            var file = new StreamWriter(path);
            var columnCount = src.TimetableFlag.GetLength(0);
            var rowCount = src.TimetableFlag.GetLength(1);
            file.Write("Tiết,");
            for (var i = 0; i < src.Classes.Count; i++)
            {
                file.Write("{0}", src.Classes[i].Name);
                file.Write(",");
            }
            file.WriteLine();

            for (int row = 1; row < rowCount; row++)
            {
                file.Write("{0}", row);
                file.Write(",");
                for (int column = 0; column < columnCount; column++)
                {
                    var unit = src.TimetableUnits.FirstOrDefault(u => u.StartAt == row && u.ClassName == src.Classes[column].Name);
                    file.Write($"{unit?.SubjectName} - {unit?.TeacherAbbreviation}");
                    file.Write(",");
                }
                file.WriteLine();
            }
            file.Close();

            file = new StreamWriter(errorPath);
            for (var i = 0; i < src.ConstraintErrors.Count; i++)
                if (src.ConstraintErrors[i].IsHardConstraint)
                    file.WriteLine("Lỗi: " + src.ConstraintErrors[i].Description);
                else
                    file.WriteLine("Lưu ý: " + src.ConstraintErrors[i].Description);
            file.Close();
        }

        public static void ToCsv(this ETimetableFlag[,] timetableFlag, List<ClassScheduleModel> classes)
        {
            var path = "D:\\WorkPlace\\CSV\\TimetableFlag.csv";
            //var path = "D:\\Workspace\\dotnet-asp\\fix\\10-be\\TimetableFlag.csv";
            var file = new StreamWriter(path);
            var columnCount = timetableFlag.GetLength(0);
            var rowCount = timetableFlag.GetLength(1);
            file.Write("Tiết/Lớp,");
            for (var i = 0; i < classes.Count; i++)
            {
                file.Write("{0}", classes[i].Name);
                file.Write(",");
            }
            file.WriteLine();

            for (int row = 1; row < rowCount; row++)
            {
                file.Write("{0}", row);
                file.Write(",");
                for (int column = 0; column < columnCount; column++)
                {
                    file.Write("{0}", timetableFlag[column, row]);
                    file.Write(",");
                }
                file.WriteLine();
            }
            file.Close();
        }

        public static void JsonOutput(this object obj, string fileName = "JsonOutput")
        {
            JsonSerializerOptions jso = new JsonSerializerOptions();
            jso.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            var json = JsonSerializer.Serialize(obj, jso);
            //var file = new StreamWriter($"C:\\Users\\ponpy\\source\\repos\\KLTN\\10-be\\{fileName}.json");
            var file = new StreamWriter($"D:\\Workspace\\dotnet-asp\\fix\\10-be\\{fileName}.json");
            file.Write(json);
            file.Close();
        }
    }
}
