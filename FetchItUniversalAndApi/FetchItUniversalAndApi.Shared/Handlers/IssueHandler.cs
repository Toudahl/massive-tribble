using System;
using System.Collections.Generic;
using System.Text;
using FetchItUniversalAndApi.Handlers.Interfaces;

namespace FetchItUniversalAndApi.Handlers
{
    class IssueHandler: ICreate, IDelete, IDisable, ISearch, ISuspend, IUpdate
    {
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

        public IEnumerable<object> Search(object obj)
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
