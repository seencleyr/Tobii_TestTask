using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Tobii_TestTask.Core
{
    /// <summary>
    /// Small description for reviewer: 
    /// This class has 2 responsibilities and break SRP from SOLID:
    /// - "task runner": run/stop execution
    /// - "queue manager": add, execute
    /// Whay I didn't split it? The main reason: this logic is tiny for splitting and from my point of view it's better to split it in "future".
    /// If Tobii project prefere in any cases use classes with only single responsible - it's not a problem for me. We can discuss this point.
    /// </summary>
    public class TaskManager
    {
        private readonly BlockingCollection<SimpleTask> _queue = new BlockingCollection<SimpleTask>(new ConcurrentQueue<SimpleTask>());
        public bool IsActive { get; private set; }
        //SpinLock - because Add method can be executed many times and we don't want to create garbage for each add
        //and also because our locked operation is fast
        static SpinLock _spinlock = new SpinLock();

        public void Run()
        {
            Task.Run(() => ExecuteTasks());
        }

        public void Stop()
        {
            IsActive = false;
            _queue.Dispose();
        }

        public void ExecuteTasks(bool infititeWaiting = true)
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;

            while (_queue.TryTake(out var currentTask, infititeWaiting ? Timeout.Infinite : 0) && IsActive) //Infinity waiting and 0 for tests
            {
                if (currentTask.State == SimpleTask.SimpleTaskState.Initialized)
                {
                    try
                    {
                        currentTask.State = SimpleTask.SimpleTaskState.Running;
                        currentTask.Action(currentTask);
                        currentTask.State = SimpleTask.SimpleTaskState.Finished;
                    }
                    catch
                    {
                        currentTask.State = SimpleTask.SimpleTaskState.Failed;
                    }
                }
                else
                {
                    currentTask.State = SimpleTask.SimpleTaskState.Skipped;
                }
            }
        }

        public SimpleTask Add(Action<SimpleTask> taskAction)
        {
            if (taskAction == null)
            {
                return null;
            }

            //After re-thinking task, I understand my mistake: 
            //the app should guarantee that task that entered "first" should also be created AND added first
            bool lockTaken = false;
            try
            {
                _spinlock.Enter(ref lockTaken);
                SimpleTask task = new SimpleTask(taskAction);
                _queue.Add(task);

                return task;
            }
            finally
            {
                if (lockTaken)
                {
                    _spinlock.Exit(false);
                }
            }
        }
    }
}