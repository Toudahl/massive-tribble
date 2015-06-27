using System.Collections.Generic;
using System.Threading.Tasks;

namespace FetchItUniversalAndApi.Handlers.Interfaces
{
    interface ISearch<T>
    {
        Task<IEnumerable<T>> Search(T obj);
    }
}
