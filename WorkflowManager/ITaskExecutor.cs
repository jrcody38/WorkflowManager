using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowManager
{
    public interface ITaskExecutor
    {
        void Execute(List<string> Parameters, ref WorkflowState workflowState);

    }
}
