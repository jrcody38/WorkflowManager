using System.Collections.Generic;

namespace WorkflowManager
{
    public class WorkflowExecutionSequence
    {
        readonly List<WorkflowTask> _tasks;
        readonly string _name;

        public WorkflowExecutionSequence(List<WorkflowTask> tasks, string name)
        {
            _tasks = tasks;
            _name = name;
        }

        public List<WorkflowTask> Tasks { get { return _tasks; } }

        public string Name{get { return _name; }}

    }
}
