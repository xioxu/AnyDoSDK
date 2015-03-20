using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnyDoDotNet;
using NUnit.Framework;

namespace AnyDoDotNet_Test
{
    [TestFixture]
    public class AnyDoDotNetClientTest
    {
        private AnyDoDotNetClient _client;
        [SetUp]
        public void LoginTest()
        {
            _client = new AnyDoDotNetClient();
            var userInfo = _client.Login("yourEmailAccount", "password");

            Assert.IsNotNull(userInfo);
            Console.WriteLine(userInfo.Name);
        }

        [Test]
        public void SubmitAndUpdateTaskTest()
        {
            var taskTitle = "API_UT";
            var task = _client.SubmitTask(new TaskCreationInfo() { DueDate = DateTime.Now.AddDays(1), Title = taskTitle });
            Assert.IsNotNull(task);
            Assert.AreEqual(taskTitle, task.Title);

            task.Status = TaskStatus.DELETED;
            task = _client.UpdateTask(task);
            Assert.AreEqual(TaskStatus.DELETED, task.Status);
        }

        [Test]
        public void GetCategoriesTest()
        {
            var categories = _client.GetCategories(false, false);
            Assert.IsNotNull(categories);
            Assert.IsTrue(categories.Length > 0);
        }

        [Test]
        public void GetTasksTest()
        {
            var tasks = _client.GetTasks(false, true);
            var taskLength = tasks.Length;
            var newTask = addTask();

            tasks = _client.GetTasks(false, true);

            Assert.IsTrue(tasks.Length == (taskLength + 1));

            newTask.Status = TaskStatus.DELETED;
            _client.UpdateTask(newTask);

            tasks = _client.GetTasks(false, true);
            Assert.IsTrue(tasks.Length == taskLength);
        }

        private TaskInfo addTask()
        {
            var taskTitle = "API_UT";
            return _client.SubmitTask(new TaskCreationInfo() { DueDate = DateTime.Now.AddDays(1), Title = taskTitle });
        }
    }
}
