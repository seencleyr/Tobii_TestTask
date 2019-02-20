using System;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tobii_TestTask.Core;

namespace Tobii_TestTask.Test
{
    [TestClass]
    public class TaskManagerTest
    {
        private static int GetCountOfQueueObject(TaskManager obj)
        {
            PrivateObject po = new PrivateObject(obj);
            return ((BlockingCollection<SimpleTask>) po.GetField("_queue")).Count;
        }

        [TestMethod]
        public void Add_AddEmptyAction_NoChangesInQueue()
        {
            //Arrange
            var taskManager = new TaskManager();

            //Act
            taskManager.Add(null);

            //Assert
            Assert.AreEqual(0, GetCountOfQueueObject(taskManager));
        }

        [TestMethod]
        public void Add_AddValidAction_NewValidTaskCreated()
        {
            //Arrange
            var taskManager = new TaskManager();
            Action<SimpleTask> action = task => { };

            //Act
            var newTask = taskManager.Add(action);

            //Assert 
            Assert.AreEqual(1, GetCountOfQueueObject(taskManager));
            Assert.AreEqual(action, newTask.Action);
            Assert.AreEqual(SimpleTask.SimpleTaskState.Initialized, newTask.State);
        }

        [TestMethod]
        public void ExecuteTasks_RunCanceledTask_TaskSkippedStatus()
        {
            //Arrange
            var taskManager = new TaskManager();
            var newTask = taskManager.Add(task => { });
            newTask.Cancel();

            //Act
            taskManager.ExecuteTasks(false);

            //Assert 
            Assert.AreEqual(SimpleTask.SimpleTaskState.Skipped, newTask.State);
        }

        [TestMethod]
        public void ExecuteTasks_RunTask_TaskFinishedStatus()
        {
            //Arrange
            var taskManager = new TaskManager();
            var newTask = taskManager.Add(task => { });

            //Act
            taskManager.ExecuteTasks(false);

            //Assert 
            Assert.AreEqual(SimpleTask.SimpleTaskState.Finished, newTask.State);
        }

        [TestMethod]
        public void ExecuteTasks_RunTaskWithException_TaskFailedStatus()
        {
            //Arrange
            var taskManager = new TaskManager();
            var newTask = taskManager.Add(task => throw new Exception());

            //Act
            taskManager.ExecuteTasks(false);

            //Assert 
            Assert.AreEqual(SimpleTask.SimpleTaskState.Failed, newTask.State);
        }
    }
}