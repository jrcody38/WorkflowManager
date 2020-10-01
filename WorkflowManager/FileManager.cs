using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace WorkflowManager
{
    public static class FileManager
    {
        private static object writeLocker = new Object();

        // full path to the file will be required here
        public static string ReadFile(string fileName)
        {
            var filePath = ConfigurationManager.AppSettings["InputFolderPath"] + "\\" + fileName + ".txt";

            using (File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var temp = File.ReadAllText(filePath);
                return temp;
            }
        }

        public static void WriteFile(string input, string fileName)
        {
            var filePath = ConfigurationManager.AppSettings["OutputFolderPath"] + "\\" + fileName + ".txt";

            lock (writeLocker)
            {
                using (StreamWriter streamPath = new StreamWriter(filePath, false))
                {
                    streamPath.WriteLine(input);
                }

            }
              
        }

        // this Grep function is case-agnostic
        public static string Grep(string inputText, string word)
        {
            string outputText = "";
            string[] fileRows = inputText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            foreach (var line in fileRows)
            {
                if (Regex.IsMatch(line.ToLower(), string.Format(@"\b{0}\b", Regex.Escape(word.ToLower()))))
                {
                    outputText += line + " \n";
                }
            }

            return outputText;

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

        public static void InitialiseOutputDirectory()
        {
            var outputFolder = ConfigurationManager.AppSettings["OutputFolderPath"];

            if (Directory.Exists(outputFolder))
            {
                purgeDirectory(outputFolder);
            }

            else
            {
                System.IO.Directory.CreateDirectory(outputFolder);
            }
        }

         static void purgeDirectory(string folderPath)
        {
            DirectoryInfo di = new DirectoryInfo(folderPath);


            for (int x = 0; x < 10; x++)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {

                    try
                    {
                        dir.Delete(true);
                    }

                    catch (Exception)
                    {
                        Thread.Sleep(1);
                        dir.Delete(true);
                    }
                }
            }
        }

    }
}
