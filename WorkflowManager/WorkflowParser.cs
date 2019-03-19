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
            List<ExecutionSequence> sequences = new List<ExecutionSequence>();
            List<WorkflowTask> workflowTasks = new List<WorkflowTask>();

            string[] fileRows = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var name = fileRows[0].Replace("desc", "").Trim();
            string currentRowContent = "";
            string parameters = "";
            int counter1 = 1;
            int counter2 = 1;


            while (currentRowContent.ToLower().Trim() != "end")
            {
                currentRowContent = fileRows[counter1];

                if(currentRowContent.ToLower().Trim() == "end")
                {
                    continue;
                }

                string[] splitRow = currentRowContent.Split('=');
                string[] rowValues = splitRow[1].Split(' ').Where( x => !string.IsNullOrEmpty(x)).ToArray();

                var task = new WorkflowTask();
                task.Id = currentRowContent.Split('=')[0].Replace("=", "").Trim();
                task.Name = rowValues[0].Split(' ')[0].Trim();

                for (int x =1; x < rowValues.Length; x++)
                {
                    var value = rowValues[x].Trim();
                    parameters += value + " ";
                }

                task.Parameter = parameters.TrimEnd();

                workflowTasks.Add(task);
                parameters = "";
                counter1++;
            }


            for (int x = counter1 + 1; x < fileRows.Length; x++)
            {
                string[] sequenceIds = fileRows[x].Split(new string[] { "->" }, StringSplitOptions.None);
                List<WorkflowTask> tempTasks = new List<WorkflowTask>();

                foreach (var id in sequenceIds)
                {
                    tempTasks.Add(workflowTasks.Where(y => y.Id == id).FirstOrDefault());
                }
                sequences.Add(new ExecutionSequence(tempTasks, "Sequence_" + counter2));
                counter1++;
                counter2++;

            }

            return new Workflow(name, sequences);
        }

    }
}
