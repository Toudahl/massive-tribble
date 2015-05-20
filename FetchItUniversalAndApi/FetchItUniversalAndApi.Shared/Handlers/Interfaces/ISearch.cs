using System.Collections.Generic;

namespace FetchItUniversalAndApi.Handlers.Interfaces
{
    interface ISearch
    {
        IEnumerable<object> Search(object obj);
    }
}
