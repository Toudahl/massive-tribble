using System;
using System.Collections.Generic;
using System.Text;

namespace FetchItUniversalAndApi.Handlers.Interfaces
{
    interface ISearch
    {
        IEnumerable<object> Search(object obj);
    }
}
