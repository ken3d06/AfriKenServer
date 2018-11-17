using System;
using System.Collections.Generic;
using System.Text;

namespace Main
{
    class WorkFlow<T>
    {
        protected List<WorkflowItem<T>> workflowItems;

        public WorkFlow()
        {
            workflowItems = new List<WorkflowItem<T>>();
        }
        public void AddItem(WorkflowItem<T> item) { workflowItems.Add(item); }

        public void Execute(T data)
        {
            WorkflowContinuation<T> continuation = new WorkflowContinuation<T>(this);
            InternalContinue(continuation, data);
        }

        public void Continue(WorkflowContinuation<T> wc, T data)
        {
            if (!wc.Abort) { wc.Defer = false; InternalContinue(wc, data); }
        }

        protected void InternalContinue(WorkflowContinuation<T> wc, T data)
        {
            while ((wc.WorkflowStep < workflowItems.Count) && !wc.Abort && !wc.Defer && !wc.Done)
            {
                WorkflowState state = workflowItems[wc.WorkflowStep++].Execute(wc, data);
                switch (state)
                {
                    case WorkflowState.Abort:
                        wc.Abort = true;
                        break;
                    case WorkflowState.Defer:
                        wc.Defer = true;
                        break;
                    case WorkflowState.Done:
                        wc.Done = true;
                        break;
                }
            }
        }
    }

    public enum WorkflowState
    {
        Abort,
        Continue,
        Defer,
        Done
    }
    internal class WorkflowItem<T>
    {
        protected Func<WorkflowContinuation<T>, T, WorkflowState> doWork;

        public WorkflowItem(Func<WorkflowContinuation<T>, T, WorkflowState> doWork)
        {
            this.doWork = doWork;
        }

        public WorkflowState Execute(WorkflowContinuation<T> workflowContinuation, T data)
        {
            return doWork(workflowContinuation, data);
        }
    }
}
