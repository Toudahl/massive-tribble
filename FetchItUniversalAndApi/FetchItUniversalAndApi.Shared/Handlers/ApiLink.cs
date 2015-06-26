using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace FetchItUniversalAndApi.Handlers
{
    class ApiLink<T>
    {
        private const string ServerUrl = "http://fetchit.mortentoudahl.dk/api/";

        public async Task<HttpResponseMessage> GetAsync()
        {
            try
            {
                using (var client = GetClient())
                {
                    return await client.GetAsync(typeof(T).Name);
                }
            }
            catch (Exception e)
            {
                new MessageDialog(e.Message).ShowAsync();
            }
            return null;
        }

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
                new MessageDialog(e.Message).ShowAsync();
            }
            return null;
        }

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
                new MessageDialog(e.Message).ShowAsync();
            }
            return null;
        }

        private HttpClient GetClient()
        {
            var client = new HttpClient {BaseAddress = new Uri(ServerUrl)};
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = new TimeSpan(0,0,30);
            return client;
        }

    }
}
