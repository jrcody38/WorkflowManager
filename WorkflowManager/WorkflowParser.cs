using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            string parameters = "";

            while (currentRowContent.ToLower().Trim() != "end")
            {
                currentRowContent = fileRows[rowCounter];

                if (currentRowContent.ToLower().Trim() == "end")
                {
                    continue;
                }

                string[] splitRow = currentRowContent.Split('=');
                string[] rowValues = splitRow[1].Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray();
                var task = new WorkflowTask();
                task.Id = currentRowContent.Split('=')[0].Replace("=", "").Trim();
                task.Name = rowValues[0].Split(' ')[0].Trim();

                for (int x = 1; x < rowValues.Length; x++)
                {
                    var value = rowValues[x].Trim();
                    parameters += value + " ";
                }

                task.Parameter = parameters.TrimEnd();
                workflowTasks.Add(task);
                parameters = "";
                rowCounter++;
            }

            return workflowTasks;
        }
        static List<ExecutionSequence> generateSequences(List<WorkflowTask> workflowTasks, string[] fileRows, int rowCounter)
        {
            List<ExecutionSequence> sequences = new List<ExecutionSequence>();
            int sequenceCounter = 1;

            for (int x = rowCounter + 1; x < fileRows.Length; x++)
            {
                string[] sequenceIds = fileRows[x].Split(new string[] { "->" }, StringSplitOptions.None);
                List<WorkflowTask> tempTasks = new List<WorkflowTask>();

                foreach (var id in sequenceIds)
                {
                    tempTasks.Add(workflowTasks.Where(y => y.Id == id).FirstOrDefault());
                }
                sequences.Add(new ExecutionSequence(tempTasks, "Sequence_" + sequenceCounter));
                sequenceCounter++;

            }

            return sequences;
           
        }
    }
}
