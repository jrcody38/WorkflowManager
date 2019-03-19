using System;
using System.Collections.Generic;
using System.IO;

namespace WorkflowManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("");
            Console.WriteLine("WORFLOW MANAGER IS RUNNING...");
            Console.WriteLine("");

            try
            {
                string inputText = "";
                using (StreamReader streamReader = new StreamReader(args[0]))
                {
                    inputText = streamReader.ReadToEnd();
                }

           
                new WorkflowHandler().Execute(WorkflowParser.ParseInput(inputText));
            }

            catch (Exception ex)
            {
                Console.WriteLine("Exception occured: " + ex.Message.ToString());
                Console.WriteLine("Press Any key to terminate...");
                Console.ReadKey();
            }

        }
    }
}
