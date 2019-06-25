using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Utils
{
    public class Guid
    {
        
        public static string GetFromAssemblyInfo(string assemblyInfoFile)
        {
            var allLines = System.IO.File.ReadAllLines(assemblyInfoFile, Encoding.UTF8);
            string _pattern = @"(\w{8}-\w{4}-\w{4}-\w{4}-\w{12})";
            var regex = new Regex(_pattern);
 

            foreach (var line in allLines)
            {
                if (line.Contains("Guid"))
                {
                    var match = regex.Match(line);
                    return match.Groups[0].ToString();
                }
            }

            throw new IndexOutOfRangeException("未能找到Guid字符串");
        }
        
        private static string GetFromTypeOfProgram(Type type)
        {
            var assembly = type.Assembly;
            var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            return attribute.Value;
        }
    }
}