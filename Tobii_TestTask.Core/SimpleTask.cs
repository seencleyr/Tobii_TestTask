using System;
using Tobii_TestTask.Core.Helper;

namespace Tobii_TestTask.Core
{
    /// <summary>
    /// Small description for reviewer: 
    /// I've created this class to provide the logic monitoring status and 
    /// also for cancelation instead for adding extra logic inside each Action.
    /// </summary>
    public class SimpleTask
    {
        public Action<SimpleTask> Action { get; }
        public long Id { get; }
        public SimpleTaskState State { get; set; }

        public enum SimpleTaskState
        {
            Initialized,
            Running,
            Finished,
            Canceled,
            Skipped,
            Failed
        }

        public SimpleTask(Action<SimpleTask> action)
        {
            Action = action;
            Id = Autoincrementor.GetNextUniqueValue();
            State = SimpleTaskState.Initialized;
        }

        public void Cancel()
        {
            State = SimpleTaskState.Canceled;
        }
    }
}