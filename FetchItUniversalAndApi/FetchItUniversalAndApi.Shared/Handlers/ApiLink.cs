using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FetchItUniversalAndApi.Handlers
{
    class ApiLink<T>
    {
        private const string ServerUrl = "http://fetchit.mortentoudahl.dk/api/";

        #region GetAsync(id)
        /// <summary>
        /// Calling this method will get all the content of the database that matches the type of <see cref="T"/>
        /// </summary>
        /// <param name="id">Enter an id, if you wish to get only a specific row from the database</param>
        /// <returns>The result of the attempted database query</returns>
        public async Task<HttpResponseMessage> GetAsync(int? id = null)
        {
            try
            {
                using (var client = GetClient())
                {
                    return await client.GetAsync(typeof(T).Name +"s/"+id);
                }
            }
            catch (Exception e)
            {
                return FailedClient(e);
            }
        }
        #endregion

        #region PostAsJsonAsync(object)
        /// <summary>
        /// Using this method will create a new row in the database, in the table that matches the type of <see cref="T"/>
        /// </summary>
        /// <param name="obj">The object to save to the database</param>
        /// <returns>The result of the attempted database query</returns>
        public async Task<HttpResponseMessage> PostAsJsonAsync(T obj)
        {
            try
            {
                using (var client = GetClient())
                {
                    return await client.PostAsJsonAsync(obj.GetType().Name + "s", obj);
                }
            }
            catch (Exception e)
            {
                return FailedClient(e);
            }
        }
        #endregion

        #region PutAsJsonAsync(object, id)
        /// <summary>
        /// This method will update the row that the object it is being passed belongs to.
        /// </summary>
        /// <param name="obj">The object to update</param>
        /// <param name="id">The id of the object</param>
        /// <returns>The result of the attempted database query</returns>
        public async Task<HttpResponseMessage> PutAsJsonAsync(T obj, int id)
        {
            try
            {
                using (var client = GetClient())
                {
                    return await client.PutAsJsonAsync(obj.GetType().Name + "s/"+id, obj);
                }
            }
            catch (Exception e)
            {
                return FailedClient(e);
            }
        }
        #endregion

        #region GetClient()
        /// <summary>
        /// Instansiates the HttpClient that is used by all the methods in this class
        /// </summary>
        /// <returns>HttpClient</returns>
        private HttpClient GetClient()
        {
            var client = new HttpClient {BaseAddress = new Uri(ServerUrl)};
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = new TimeSpan(0,0,30);
            return client;
        }
        #endregion

        #region FailedClient
        /// <summary>
        /// Any method that returns a HttpResponseMessage will return this upon an exception
        /// </summary>
        /// <returns>HttpStatusCode.NoContent</returns>
        private HttpResponseMessage FailedClient(Exception e)
        {
            var response = new HttpResponseMessage();
            response.ReasonPhrase = e.Message;
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
        #endregion
    }
}
