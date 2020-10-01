using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowManager
{
    public class Workflow
    {
        readonly List<WorkflowExecutionSequence> _sequences;
        readonly string _name;

        public Workflow(string name, List<WorkflowExecutionSequence> sequences)
        {
            _name = name;
            _sequences = sequences;
        }
       public List<WorkflowExecutionSequence> Sequences
        {
            get { return _sequences; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
