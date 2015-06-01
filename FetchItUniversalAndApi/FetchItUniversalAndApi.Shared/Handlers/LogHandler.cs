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
    /// <summary>
    /// Handles creating Logs when a system error has occured
    /// Should in most situations be only used through the ErrorHandler
    /// </summary>
    class LogHandler: ICreate
    {
        #region Fields and Properties
        //A field containing a reference to this class to be returned using GetInstance()
        private static LogHandler _handler;
        //A object being used to lock the thread while instanciating the class
        private static Object _lockObject = new object();
        //The HttpClient used for doing POST, GET and etc. commands to the web api.
        //Differs from other handlers since it's not garbage collected to close the connection
        //According to my research it should handle opening/closing connections automatically
        //This mainly means better performance and shorter code
        private static HttpClient logWebClient = new HttpClient();
        //The url for the web Api. Ensured that it does not change.
        private static readonly string serverLocation = "http://fetchit.mortentoudahl.dk/api/";
        #endregion

        #region Methods
        #region Constructor
        /// <summary>
        /// Constructor. Sets the properties of the HttpClient being used by the loghandler
        /// Private to ensure proper singleton behavior
        /// </summary>
        private LogHandler()
        {
            logWebClient.BaseAddress = new Uri(serverLocation);
            logWebClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        #endregion

        #region Singleton Implementation
        /// <summary>
        /// Get an instance of the <see cref="LogHandler"/>
        /// This singleton is Thread safe
        /// </summary>
        /// <returns>LogHandler object</returns>
        public static LogHandler GetInstance()
        {
            //Locks the thread
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
        /// Should in most cases only be used by <see cref="ErrorHandler"/>.
        /// Creates a Log object in the database adding the time created and posting the <see cref="LogModel"/>
        /// </summary>
        /// <param name="objectBeingLogged">A LogModel object</param>
        public void Create(object objectBeingLogged)
        {
            if (objectBeingLogged != null)
            {
                if (objectBeingLogged is LogModel)
                {
                    var sendingLog = objectBeingLogged as LogModel;
                    sendingLog.LogTime = DateTime.UtcNow;
                    try
                    {
                        logWebClient.PostAsJsonAsync("LogModels", sendingLog);
                    }
                    catch (Exception)
                    {
                        //For now we suppress the errors
                        //If we'd call the ErrorHandler here we'd have an infinite loop
                        //We do not want the user experience to be affected when we are unable to log system errors
                    }
                }
            }
        }

        #region GetLog() Overloads (some are not implemented)
        /// <summary>
        /// NOT IMPLEMENTED Might restructure the GetLog overloads to get logs after/before and for a certain amount of time.
        /// </summary>
        /// <param name="duration">The timespan you wish to go back in time</param>
        /// <returns>An IENumerable of LogModels</returns>
        public IEnumerable<LogModel> GetLog(TimeSpan duration)
        {
            throw new NotImplementedException("Not Implemented: See if you can use GetLog(string) instead. Author: Lárus Þór");
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

        /// <summary>
        /// Get logs where the LogMessage contains the string parameter
        /// </summary>
        /// <param name="phraseToSearchFor">The string you want to search for (profile name, log type and etc.)</param>
        /// <returns>An IENumerable of LogModels</returns>
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
                ErrorHandler.NoResponseFromApi();
                return null;
            }
        }

        /// <summary>
        /// NOT IMPLEMENTED Might restructure the GetLog overloads to get logs after/before and for a certain amount of time.
        /// </summary>
        /// <param name="dateTime">The date which you want to get logs from</param>
        /// <returns>An IENumerable of LogModels</returns>
        public IEnumerable<LogModel> GetLog(DateTime dateTime)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}
