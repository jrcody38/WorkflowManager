using System.Collections.Generic;

namespace WorkflowManager.Tasks
{
    public class ReadFile : ITaskExecutor
    {
        public void Execute(List<string> parameters, ref WorkflowState workflowState)
        {
            var name = parameters[0];
            workflowState.CurrentText = FileManager.ReadFile(name);
        }
    }
}
