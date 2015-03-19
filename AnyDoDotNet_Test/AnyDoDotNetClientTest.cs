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
            var userInfo = _client.Login("52280764@qq.com", "a111111");

            Assert.IsNotNull(userInfo);
            Console.WriteLine(userInfo.Name);
        }

        [Test]
        public void SubmitAndUpdateTaskTest()
        {
            var taskTitle = "API_UT";
            var task = _client.SubmitTask(new TaskCreationInfo(){DueDate = DateTime.Now.AddDays(1), Title = taskTitle });
            Assert.IsNotNull(task);
            Assert.AreEqual(taskTitle, task.Title);

            task.Status = TaskStatus.DELETED;
            task = _client.UpdateTask(task);
            Assert.AreEqual(TaskStatus.DELETED,task.Status);
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
            var tasks = _client.GetTasks(true, true);
            Assert.IsNotNull(tasks);
            Assert.IsTrue(tasks.Length > 0);
        }
    }
}
