using System;
using WorkflowManager.Tasks;

namespace WorkflowManager
{
    public class WorkflowState
    {
        public string CurrentText { get; set; }

        public string ExecutionSequence { get; set; }

        public event EventHandler<SynchEventArgs> SyncTriggered;

       public  void OnSyncTriggered(SynchEventArgs e)
        {
            EventHandler<SynchEventArgs> handler = SyncTriggered;
            handler?.Invoke(this, e);
        }

    }
}
