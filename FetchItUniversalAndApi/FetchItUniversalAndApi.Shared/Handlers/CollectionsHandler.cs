using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FetchItUniversalAndApi.Handlers
{
    //Author: Lárus Þór Masterson, Morten Toudahl
    public static class CollectionsHandler
    {
        /// <summary>
        /// Takes an IENumerable (default return collection value of handlers) and casts it to an ObservableCollection (default collection used in viewmodels)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerableList"></param>
        /// <returns></returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerableList)
        {
            return new ObservableCollection<T>(enumerableList);
        }
    }
}
