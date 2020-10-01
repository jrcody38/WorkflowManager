using System.Collections.Generic;

namespace WorkflowManager
{
    public class WorkflowTask
    {
        readonly string _id;
        readonly List<string> _parameters;
        readonly ITaskExecutor _executor;


        public WorkflowTask(string id, List<string> parameters, ITaskExecutor executor)
        {
            _id = id;
            _parameters = parameters;
            _executor = executor;
        }

        public List<string> Parameters
        {
            get { return _parameters; }
        }

        public string Id
        {
            get { return _id; }
        }


        public void Execute(ref WorkflowState state)
        {
            _executor.Execute(_parameters, ref state);
        }

    }
}
