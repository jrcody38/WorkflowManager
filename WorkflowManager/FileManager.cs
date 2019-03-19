using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WorkflowManager
{
    public static class FileManager
    {
        // full path to the file will be required here
        public static string ReadFile(string filePath)
        {
            using (File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var temp = File.ReadAllText(filePath);
                return temp;
            }
        }

        public static void WriteFile(string input, string fileName)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "\\" + ConfigurationManager.AppSettings["OutputFolder"]+ "\\" + fileName + ".txt";

            using (StreamWriter streamPath = new StreamWriter(path, false))
            {
                streamPath.WriteLine(input);
            }
        }

        // this Grep function is case-agnostic
        public static string Grep(string inputText, string word)
        {
            string outputText = "";
            string[] fileRows = inputText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            foreach (var line in fileRows)
            {
                if(Regex.IsMatch(line.ToLower(), string.Format(@"\b{0}\b", Regex.Escape(word.ToLower()))))
                {
                    outputText += line + " \n";
                }
            }

            return  outputText;
            
        }

        // this Sort function takes whitespaces into account in the sorting. Not sure if that is what is wanted but I left it like that. 
        public static string Sort(string inputText)
        {
            string outputText = "";
            string[] fileRows = inputText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
       
            fileRows = fileRows.OrderBy(x => x).ToArray();

            foreach (var s in fileRows)
            {
                outputText += s + " \n";
            }

            return outputText;
        }

        // this Replace function function is CASE-SENSITIVE
        public static string Replace(string inputText, string toReplace, string newWord)
        {
            string outputText = "";
            string[] fileRows = inputText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            
            for (int x = 0; x < fileRows.Length; x++)
            {
                fileRows[x] = Regex.Replace(fileRows[x], string.Format(@"\b{0}\b", toReplace), newWord);
            }

            foreach (var s in fileRows)
            {
                outputText += s + " \n";
            }

            return outputText;
        }
    }
}
