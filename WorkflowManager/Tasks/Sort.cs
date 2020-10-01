using System.Collections.Generic;

namespace WorkflowManager.Tasks
{
    public class Sort : ITaskExecutor
    {
        public void Execute(List<string> Parameters, ref WorkflowState workflowState)
        {
            string inputText = workflowState.CurrentText;
            workflowState.CurrentText = FileManager.Sort(inputText);
        }
    }
}
