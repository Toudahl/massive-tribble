using System;
using System.Collections.Generic;
using System.Text;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers
{
    class LogHandler: ICreate
    {
        private static LogHandler _handler;
        private static Object _lockObject = new object();


        private LogHandler()
        {
            
        }

        public static LogHandler GetInstance()
        {
            lock (_lockObject)
            {
                if (_handler == null)
                {
                    _handler = new LogHandler();
                }
                return _handler;
            }
        }

        public void Create(object obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LogModel> GetLog(TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LogModel> GetLog(string phraseToSearchFor)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LogModel> GetLog(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

    }
}
