using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FetchItUniversalAndApi.Handlers
{
    public class TaskHandler : ICreate, IDelete, IDisable, ISuspend, ISearch, IUpdate
    {
        private const string taskAPI = "http://fetchit.mortentoudahl.dk/api/TaskModels";

        private TaskModel _selectedTask;
        private TaskModel _newTask;
        private static TaskHandler _handler;
        private static Object _lockObject = new object();

        // Must be set to the same value that the db uses to reference the status.
        // ie: Active = 1, Suspended = 2; etc
        public enum TaskStatus
        {
            Active = 1,
            Reported = 2,
            Removed = 3,
            Deleted = 4,
            Completed = 5
        }

        private TaskHandler()
        {
            //TODO Add TaskTitle
        }

        public static TaskHandler GetInstance()
        {
            lock (_lockObject)
            {
                if (_handler == null)
                {
                    _handler = new TaskHandler();
                }
                return _handler;
            }
        }

        public TaskModel NewTask
        {
            get { return _newTask; }
            set { _newTask = value; }
        }


        public async void Create(object taskObject)
        {
            if (taskObject is TaskModel)
            {
                var addNewTask = new TaskModel();
                addNewTask.FK_TaskMaster = 11;
                addNewTask.FK_TaskStatus = (int)TaskStatus.Active;

                using (var Client = new HttpClient())
                {
                    await Client.PostAsJsonAsync(taskAPI, taskObject);
                }
            }

            //throw new NotImplementedException();
        }


        public async void Delete(object taskObject)
        {
            if (taskObject is TaskModel) // ADD THE IF PROFILE LEVEL DELETEING IS ADMINISTRATOR
            {
                var taskToDelete = taskObject as TaskModel;
                using (var Client = new HttpClient())
                {
                    var url = taskAPI + "/" + taskToDelete.TaskId;
                    taskToDelete.FK_TaskStatus = (int)TaskStatus.Deleted;
                    try
                    {
                        await Client.PutAsJsonAsync(url, taskToDelete);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }

            //throw new NotImplementedException();
        }

        public async void Remove(object taskObject)
        {
            if (taskObject is TaskModel) // ADD THE IF PROFILE IS CURRENT LOGGED IN PROFILE
            {
                var taskToRemove = taskObject as TaskModel;
                using (var Client = new HttpClient())
                {
                    var url = taskAPI + "/" + taskToRemove.TaskId;
                    taskToRemove.FK_TaskStatus = (int)TaskStatus.Removed;
                    try
                    {
                        await Client.PutAsJsonAsync(url, taskToRemove);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
        }

        public async void Report(object taskObject)
        {
            if (taskObject is TaskModel)
            {
                var taskToReport = taskObject as TaskModel;
                using (var Client = new HttpClient())
                {
                    var url = taskAPI + "/" + taskToReport.TaskId;
                    taskToReport.FK_TaskStatus = (int)TaskStatus.Reported;
                    try
                    {
                        await Client.PutAsJsonAsync(url, taskToReport);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
        }

        public async void Complete(object taskObject)
        {
            if (taskObject is TaskModel)
            {
                var taskToComplete = taskObject as TaskModel;
                using (var Client = new HttpClient())
                {
                    var url = taskAPI + "/" + taskToComplete.TaskId;
                    taskToComplete.FK_TaskStatus = (int)TaskStatus.Completed;
                    try
                    {
                        await Client.PutAsJsonAsync(url, taskToComplete);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
        }

        public void Disable(object obj)
        {
            throw new NotImplementedException();
        }

        public void Suspend(object obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> Search(object obj)
        {
            throw new NotImplementedException();
        }

        public void Update(object taskObject)
        {
            //var taskToUpdate = taskObject as TaskModel;
            //if (taskToUpdate != null)
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress
            //    }
            //}
        }

        public TaskModel SelectedTask
        {
            get { return _selectedTask; }
            set { _selectedTask = value; }
        }

        public IEnumerable<TaskModel> GetTasks(TaskStatus value)
        {
            if (value == TaskStatus.Active)
            {
                IEnumerable<TaskModel> getAll;
                using (var client = new HttpClient())
                {
                    getAll = Task.Run(async () => JsonConvert.DeserializeObject<IEnumerable<TaskModel>>(
                                    await client.GetStringAsync(taskAPI))).Result;
                }
                return getAll;
            }

            throw new NotImplementedException();
        }
    }
}