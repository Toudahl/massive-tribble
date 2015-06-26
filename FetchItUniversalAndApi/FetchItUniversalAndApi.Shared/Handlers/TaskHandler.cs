using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;
using Newtonsoft.Json;

namespace FetchItUniversalAndApi.Handlers
{
    // Author : Bruno Damjanovic
    /// <summary>
    /// This is the 
    /// </summary>
    public class TaskHandler : ICreate<TaskModel>, IDelete, IDisable, ISuspend, ISearch, IUpdate
    {
        private const string taskAPI = "http://fetchit.mortentoudahl.dk/api/TaskModels";

        private TaskModel _selectedTask;
        private TaskModel _newTask;
        private static TaskHandler _handler;
        private ProfileHandler _ph;
        private static Object _lockObject = new object();

        #region TaskStatus enums
        public enum TaskStatus
        {
            All = 0,
            Active = 1,
            Reported = 2,
            Removed = 3,
            Deleted = 4,
            Completed = 5,
			TaskMasterCompleted = 6,
			FetcherCompleted = 7
        }
        #endregion

        #region Properties
        ///<summary>
        /// We have forgotten to create a Title property for the task. 
        /// In order to do it now, we would have to update the database and the API and all of the diffrent branches 
        /// We have agreed that it would be best to leave it to be done after the merging.
        /// 
        /// //TODO Add TaskTitle
        /// //TODO Task Location
        /// </summary>

        private TaskHandler()
        {
            _ph = ProfileHandler.GetInstance();
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

        public TaskModel SelectedTask
        {
            get { return _selectedTask; }
            set { _selectedTask = value; }
        }
        #endregion

        //Methods that the TaskHandler utilizes

        #region Create Task

        /// <summary>
        /// Creates a new TaskModel object.
        /// </summary>
        /// <param name="taskObject"></param>

        public async void Create(TaskModel taskObject)
        {
            if (taskObject == null) return;
                taskObject.FK_TaskMaster = _ph.CurrentLoggedInProfile.ProfileId;
                taskObject.FK_TaskStatus = (int)TaskStatus.Active;
                taskObject.TaskTimeCreated = DateTime.UtcNow;

                using (var Client = new HttpClient())
                {
                    try
                    {
                        await Client.PostAsJsonAsync(taskAPI, taskObject);
                    }
                    catch (Exception)
                    {
                        ErrorHandler.NoResponseFromApi();
                    }
                }
        }
        #endregion

        #region Delete Task
        /// <summary>
        /// Delete method changes the TaskStatus to deleted.
        /// It can only be done by the Administrator level profile, so it also makes the check, and if it is not 
        /// </summary>
        /// <param name="taskObject"></param>

        public async void Delete(object taskObject)
        {
            if (taskObject is TaskModel)
            {
                var taskToDelete = taskObject as TaskModel;
                if (_ph.CurrentLoggedInProfile.FK_ProfileLevel >= (int)ProfileHandler.ProfileLevel.Administrator)
                {
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
                            ErrorHandler.NoResponseFromApi();
                        }
                    }
                }
            }
        }
        #endregion

        #region Remove Task
        /// <summary>
        /// This method changes the status of the task to Removed. It can only be performed by the TaskMaster profile
        /// It checks if the CurrentLoggedInProfile is the sam profile that has posted the task. if that is true, the status can be changed to removed.
        /// </summary>
        /// <param name="taskObject"></param>


        public async void Remove(object taskObject)
        {
            if (taskObject is TaskModel)
            {
                var taskToRemove = taskObject as TaskModel;
                if (ProfileHandler.GetInstance().CurrentLoggedInProfile.ProfileId == taskToRemove.FK_TaskMaster)
                {
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
                            ErrorHandler.NoResponseFromApi();
                        }
                    }
                }
                else
                {
                    
                }
            }
            else
            {
                
            }
        }
        #endregion

        #region Report Task
        /// <summary>
        /// A method that changes the TaskStatus to Reported
        /// Makes sure that the current logged in profile is NOT the profile to report so a profile does not report his own Task
        /// </summary>
        /// <param name="taskObject"></param>

        public async void Report(object taskObject)
        {
            if (taskObject is TaskModel)
            {
                var taskToReport = taskObject as TaskModel;
                if (taskToReport.FK_TaskMaster != ProfileHandler.GetInstance().CurrentLoggedInProfile.ProfileId)
                {
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
                            ErrorHandler.NoResponseFromApi();
                        }
                    }
                }
                else
                {
                    
                }
            }
            else
            {
                
            }
        }
        #endregion

        #region Complete Task
        /// <summary>
        /// Complete method allows for a task to be completed.
        /// In order to do that, both Taskmaster and Fetcher have to complete the task.
        /// We make the method go through a series checks in order t+o find out if a fetcher or a taskamster has already clicked "Finish"
        /// If not, then it changes to a "half-finished" state, if it has, then the status changes to Completed
        /// </summary>
        /// <param name="taskObject"></param>

        public async void Complete(object taskObject)
        {
            if (taskObject is TaskModel)
            {
                var taskToComplete = taskObject as TaskModel;
                using (var Client = new HttpClient())
                {
                    if (taskToComplete.FK_TaskStatus == (int)TaskStatus.FetcherCompleted)
                    {
                        setStatus(taskToComplete.FK_TaskMaster, TaskStatus.Completed, taskToComplete);
                    }
                    else if (taskToComplete.FK_TaskStatus == (int)TaskStatus.TaskMasterCompleted)
                    {
                        setStatus(taskToComplete.FK_TaskFetcher, TaskStatus.Completed, taskToComplete);
                    }
                    else if (taskToComplete.FK_TaskStatus == (int)TaskStatus.Active)
                    {
                        setStatus(taskToComplete.FK_TaskFetcher, TaskStatus.FetcherCompleted, taskToComplete);
                        setStatus(taskToComplete.FK_TaskMaster, TaskStatus.TaskMasterCompleted, taskToComplete);
                    }
                    var url = taskAPI + "/" + taskToComplete.TaskId;
                    try
                    {
                        await Client.PutAsJsonAsync(url, taskToComplete);
                    }
                    catch (Exception)
                    {
                        ErrorHandler.NoResponseFromApi();
                    }
                }
            }
        }
        
        /// <summary>
        /// setStatus method allows us to minimize redundant rode repetition inside of the Complete method.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="status"></param>
        /// <param name="taskToComplete"></param>

        private void setStatus(int? ID, TaskStatus status, TaskModel taskToComplete)
        {
            if (ProfileHandler.GetInstance().CurrentLoggedInProfile.ProfileId == ID)
            {
                taskToComplete.FK_TaskStatus = (int)status;
            }
        }
        #endregion

        #region Disable Task
        public void Disable(object obj)
        {
            throw new NotImplementedException();
        }
        #endregion 

        #region Suspend Task
        public void Suspend(object obj)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Search Tasks
        /// <summary>
        /// This method searches for specific tasks in the marketplace, but only if their Taskmaster is CurrentLoggedInProfile
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IEnumerable<object> Search(object obj)
        {
            if (obj is TaskModel)
            {
                var taskToSearchFor = obj as TaskModel;
                IEnumerable<TaskModel> marketplace;
                using (var client = new HttpClient())
                {
                    marketplace = Task.Run(
                        async () =>
                            JsonConvert.DeserializeObject<IEnumerable<TaskModel>>(await client.GetStringAsync(taskAPI)))
                        .Result;
                }
                if (taskToSearchFor.TaskId != 0)
                {
                    return marketplace.Where(task => task.TaskId == taskToSearchFor.TaskId);  
                }

                if (taskToSearchFor.FK_TaskMaster != 0)
                {
                    return marketplace.Where(task => task.FK_TaskMaster == taskToSearchFor.FK_TaskMaster);                    
                }
                
            }
            return null;
        }
        #endregion

        #region Update Task
        /// <summary>
        /// This method allows for a Task to be Updated (Edit task)
        /// </summary>
        /// <param name="taskObject"></param>

        async public void Update(object taskObject)
        {
            if (taskObject is TaskModel)
            {
                var taskToUpdate = taskObject as TaskModel;
                using (var Client = new HttpClient())
                {
                    var url = taskAPI + "/" + taskToUpdate.TaskId;
                    try
                    {
                        await Client.PutAsJsonAsync(url, taskToUpdate);
                    }
                    catch (Exception)
                    {
                        ErrorHandler.NoResponseFromApi();
                    }
                }
            }
            else
            {

            }
        }
        #endregion

        #region Get Tasks
        /// <summary>
        /// GetTasks returns tasks of the supplied Status (All, Active, Reported...)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>

        public IEnumerable<TaskModel> GetTasks(TaskStatus value)
        {
            IEnumerable<TaskModel> getAll;
            using (var client = new HttpClient())
            {
                getAll = Task.Run(async () => JsonConvert.DeserializeObject<IEnumerable<TaskModel>>(
                await client.GetStringAsync(taskAPI))).Result;
            }
            switch (value)
            {
                case TaskStatus.All: return getAll;
                case TaskStatus.Active: return getAll.Where(s => s.FK_TaskStatus == 1);
                case TaskStatus.Reported: return getAll.Where(s => s.FK_TaskStatus == 2);
                case TaskStatus.Removed: return getAll.Where(s => s.FK_TaskStatus == 3);
                case TaskStatus.Deleted: return getAll.Where(s => s.FK_TaskStatus == 4);
                case TaskStatus.Completed: return getAll.Where(s => s.FK_TaskStatus == 5);
                case TaskStatus.TaskMasterCompleted: return getAll.Where(s => s.FK_TaskStatus == 6);
                case TaskStatus.FetcherCompleted: return getAll.Where(s => s.FK_TaskStatus == 7);
                default:
                    return null;
            }
        }
        #endregion
    }
}