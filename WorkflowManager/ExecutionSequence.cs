using System;
using System.Collections.Generic;
using System.Text;


namespace WorkflowManager
{
    public class ExecutionSequence
    {
        List<WorkflowTask> _tasks;
        string _currentText;
        string _name;

        public ExecutionSequence(List<WorkflowTask> tasks, string name)
        {
            _tasks = tasks;
            _name = name;
        }

        public List<WorkflowTask> Tasks { get { return _tasks; } }

        public string CurrentText { get { return _currentText; } set { _currentText = value; } }

        public string Name{get { return _name; }}

    }
}
