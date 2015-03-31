using System;
using System.Collections.Generic;
using System.Text;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers
{
    class ReportHandler: ICreate, IDelete, IDisable, ISuspend, IUpdate
    {
        private static Object _lockObject = new object();
        private static ReportHandler _handler;
        // Must be set to the same values as the values in the db.
        public enum ReportStatus
        {
            
        }

        private ReportHandler()
        {
            
        }

        public static ReportHandler GetInstance()
        {
            lock (_lockObject)
            {
                if (_handler == null)
                {
                    _handler = new ReportHandler();
                }
                return _handler;
            }
        }

        public static IEnumerable<ReportModel> GetReports(ReportStatus status)
        {
            throw new NotImplementedException();
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

        public void Update(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
