using System.Collections.Generic;

namespace WorkflowManager.Tasks
{
    public class Grep : ITaskExecutor    
    {
       
        public void Execute(List<string> parameters, ref WorkflowState workflowState)
        {
            string inputText = workflowState.CurrentText;
            string word = parameters[0];
            workflowState.CurrentText = FileManager.Grep(inputText, word);
        }
    }
}
