using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers
{
    class TaskHandler: ICreate, IDelete, IDisable, ISuspend, ISearch, IUpdate
    {
        private TaskModel _selectedTask;
        private static TaskHandler _handler;
        private static Object _lockObject = new object();

        // Must be set to the same value that the db uses to reference the status.
        // ie: Active = 1, Suspended = 2; etc
        public enum StatusOfTask
        {
            
        }

        private TaskHandler()
        {
            
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


        public void Create(object obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(object obj)
        {
            throw new NotImplementedException();
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

        public void Update(object obj)
        {
            throw new NotImplementedException();
        }

        public TaskModel SelectedTask
        {
            get { return _selectedTask; }
            set { _selectedTask = value; }
        }

        public IEnumerable<TaskModel> GetTasks(StatusOfTask value)
        {
            throw new NotImplementedException();
        }
    }
}
