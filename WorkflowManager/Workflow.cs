using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowManager
{
    public class Workflow
    {
        List<ExecutionSequence> _sequences;
        string _name;



        public Workflow(string name, List<ExecutionSequence> sequences)
        {
            _name = name;
            _sequences = sequences;
        }
       public List<ExecutionSequence> Sequences
        {
            get { return _sequences; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
