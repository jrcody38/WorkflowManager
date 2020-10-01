using System;
using System.Collections.Generic;

namespace WorkflowManager.Tasks
{
    public class Synch: ITaskExecutor
    {
       
        public void Execute(List<string> parameters, ref WorkflowState workflowState)
        {
            SynchEventArgs args = new SynchEventArgs();
            args.CurrentSequence = workflowState.ExecutionSequence;
            workflowState.OnSyncTriggered(args);
        }
    }


    public class SynchEventArgs : EventArgs
    {
        public string CurrentSequence { get; set; }

    }
}
