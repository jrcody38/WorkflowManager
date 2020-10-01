using System.Collections.Generic;

namespace WorkflowManager.Tasks
{
    public class Replace : ITaskExecutor
    {
        public void Execute(List<string> parameters, ref WorkflowState workflowState)
        {
            string inputText = workflowState.CurrentText;
            string toReplace = parameters[0];
            string newWord = parameters[1];

            workflowState.CurrentText = FileManager.Replace(inputText, toReplace, newWord);

        }
    }
}
