using System;
using System.Collections.Generic;
using System.Text;

namespace Main
{
    /// <summary>
    /// Thread-specific instance that preserves the workflow continuation context for that thread
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class WorkflowContinuation<T>
    {
        public int WorkflowStep { get; set; }
        public bool Abort { get; set; }
        public bool Defer { get; set; }
        public bool Done { get; set; }
        public  WorkFlow<T> WorkFlow { get; protected set; }

        public WorkflowContinuation(WorkFlow<T> workFlow)
        {
            WorkFlow = workFlow;
        }
    }
}
