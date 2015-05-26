using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;
using Newtonsoft.Json;

namespace FetchItUniversalAndApi.Handlers
{
    //Author: Lárus Þór Óðinsson
    class LogHandler: ICreate
    {
        #region Fields and Properties
        private static LogHandler _handler;
        private static Object _lockObject = new object();
        //The httpclient should probably be one object that all handlers call upon. There "shouldn't" be any reason to dispose of it or flush it.
        //But we should keep an eye out for if it's not closing the connections or not dumping the resources.
        private static HttpClient logWebClient = new HttpClient();
        //TODO: This could probably be somewhere better and globally available. Also needs to be changed to refer to the intended server.
        private static readonly string serverLocation = "http://fetchit.mortentoudahl.dk/api/";
        #endregion

        #region Methods
        #region Constructor
        private LogHandler()
        {
            logWebClient.BaseAddress = new Uri(serverLocation);
            logWebClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        #endregion

        #region Singleton Implementation
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
        #endregion

        // Bugfix by : Morten Toudahl
        /// <summary>
        /// Creates a Log object in the database adding the time created and posting the <see cref="LogModel"/>
        /// </summary>
        /// <param name="obj"></param>
        public void Create(object objectBeingLogged)
        {
            if (objectBeingLogged != null)
            {
                if (objectBeingLogged is LogModel)
                {
                    var sendingLog = objectBeingLogged as LogModel;
                    sendingLog.LogTime = DateTime.UtcNow;
                    //sendingLog.LogMessage = objectBeingLogged.ToString();
                    try
                    {
                        logWebClient.PostAsJsonAsync("LogModels", sendingLog);
                    }
                    catch (Exception)
                    {
                        //For now we suppress the errors
                    }
                }
            }
        }

        /// <summary>
        /// NOT IMPLEMENTED Might restructure the GetLog overloads to get logs after/before and for a certain amount of time.
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public IEnumerable<LogModel> GetLog(TimeSpan duration)
        {
            try
            {
                IEnumerable<LogModel> haystack = Task.Run(
                         async () =>
                             JsonConvert.DeserializeObject<IEnumerable<LogModel>>(await logWebClient.GetStringAsync("LogModels"))).Result;
                //return haystack.Where(l => l.LogTime timeSpan.);
            }
            catch (Exception exception)
            {
                //Add standardized error handling (fx. LogHandler.GetInstance().LogEvent(exception.message) and MessageBox.Show("Yo user, something went wrong!"));
                throw exception;
            }
            return null;
        }

        public IEnumerable<LogModel> GetLog(string phraseToSearchFor)
        {
            try
            {
                IEnumerable<LogModel> haystack = Task.Run(
                         async () =>
                             JsonConvert.DeserializeObject<IEnumerable<LogModel>>(await logWebClient.GetStringAsync("LogModels"))).Result;
                    return haystack.Where(l => l.LogMessage.Contains(phraseToSearchFor));
            }
            catch (Exception exception)
            {
                //Add standardized error handling (fx. LogHandler.GetInstance().LogEvent(exception.message) and MessageBox.Show("Yo user, something went wrong!"));
                throw exception;
            }
        }

        public IEnumerable<LogModel> GetLog(DateTime dateTime)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
