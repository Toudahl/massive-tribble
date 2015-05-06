using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FetchItUniversalAndApi.Handlers
{
    public static class CollectionsHandler
    {
            public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerableList)
            {
                return new ObservableCollection<T>(enumerableList);
                //if (enumerableList != null)
                //{
                //    //create an emtpy observable collection object
                //    var observableCollection = new ObservableCollection<T>();

                //    //loop through all the records and add to observable collection object
                //    foreach (var item in enumerableList)
                //        observableCollection.Add(item);

                //    //return the populated observable collection
                //    return observableCollection;
                //}
                //return null;
            }
    }
}
