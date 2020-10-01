using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkflowManager
{
    public static class WorkflowParser
    {
        public static Workflow ParseInput(string input)
        {
            string[] fileRows = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var name = fileRows[0].Replace("desc", "").Trim();
            int rowCounter = 1;
            
            return new Workflow(name, generateSequences(generateWorkflowTasks(fileRows, ref rowCounter), fileRows, rowCounter));
        }

        static List<WorkflowTask> generateWorkflowTasks(string[] fileRows, ref int rowCounter)
        {
            List<WorkflowTask> workflowTasks = new List<WorkflowTask>();
          
            string currentRowContent = "";
            

            while (currentRowContent.ToLower().Trim() != "end")
            {
              List<string> parameters = new List<string>();
              currentRowContent = fileRows[rowCounter];

                if (currentRowContent.ToLower().Trim() == "end")
                {
                    continue;
                }

                string[] splitRow = currentRowContent.Split('=');
                string[] rowValues = splitRow[1].Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray();
               
                var id = currentRowContent.Split('=')[0].Replace("=", "").Trim();
                var name = rowValues[0].Split(' ')[0].Trim();

                for (int x = 1; x < rowValues.Length; x++)
                {
                    var value = rowValues[x].Trim();
                    parameters.Add(value);
                    
                }

                var task = new WorkflowTask(id, parameters, getExecutor(name));
                workflowTasks.Add(task);
              
                rowCounter++;
            }

            return workflowTasks;
        }
        static List<WorkflowExecutionSequence> generateSequences(List<WorkflowTask> workflowTasks, string[] fileRows, int rowCounter)
        {
            List<WorkflowExecutionSequence> sequences = new List<WorkflowExecutionSequence>();
            int sequenceCounter = 1;

            for (int x = rowCounter + 1; x < fileRows.Length; x++)
            {
                string[] sequenceIds = fileRows[x].Split(new string[] { "->" }, StringSplitOptions.None);
                List<WorkflowTask> tempTasks = new List<WorkflowTask>();

                foreach (var id in sequenceIds)
                {
                    tempTasks.Add(workflowTasks.Where(y => y.Id == id).FirstOrDefault());
                }
                sequences.Add(new WorkflowExecutionSequence(tempTasks, "Sequence_" + sequenceCounter));
                sequenceCounter++;

            }

            return sequences;
           
        }


        static ITaskExecutor getExecutor(string name)
         {
            Type type = Type.GetType("WorkflowManager.Tasks." + name, true, true);
            return (ITaskExecutor)Activator.CreateInstance(type);
         }
    }
}
