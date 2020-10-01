using System.Collections.Generic;

namespace WorkflowManager.Tasks
{
    class WriteFile : ITaskExecutor
    {
        public void Execute(List<string> parameters, ref WorkflowState workflowState)
        {
            string fileName = parameters[0];
            string input = workflowState.CurrentText;

            FileManager.WriteFile(input, fileName);
        }
    }
}
