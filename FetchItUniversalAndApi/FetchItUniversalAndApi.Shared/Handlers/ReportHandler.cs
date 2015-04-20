using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers
{
    //Author: Kristinn Þór Jónsson
    /// <summary>
    /// Handles getting, deleting, creating and updating Reports.
    /// </summary>
    class ReportHandler: ICreate, IDelete, IDisable, ISuspend, IUpdate
    {
        private static readonly string _serverUrl = "http://fetchit.mortentoudahl.dk/api/";
        private static Object _lockObject = new object();
        private static ReportHandler _handler;
        private static HttpClient Client { get; set; }

        // Must be set to the same values as the values in the db.
        public enum ReportStatus
        {
            Active,
            Suspended,
            Disabled,
            Deleted,
        }

        private ReportHandler()
        {

        }

        public static ReportHandler GetInstance()
        {
            lock (_lockObject)
            {
                if (_handler == null)
                {
                    _handler = new ReportHandler();
                }
                return _handler;
            }
        }

        /// <summary>
        /// Gets all the report objects from the database that have the specified report status.
        /// </summary>
        /// <param name="status">The type of status to return.</param>
        /// <returns></returns>
        public IEnumerable<ReportModel> GetReports(ReportStatus status)
        {
            using (Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(_serverUrl);
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    //Does not use the ReportStatusId at this point, now it just returns all the reports from the database.
                    //TODO: Find a way to use the Report Status to get the specified reports.
                    var reports = Task.Run(async () => await Client.GetAsync("reportmodels"));
                    return reports.Result.Content.ReadAsAsync<IEnumerable<ReportModel>>().Result;
                    
                    //TODO: Possibly find another way to use the Report Status to get the specified reports.
                    //Through something called Attribute routing, it is easily possible to make the 
                    //url "reportmodels/status/1" return all statuses numbered 1, etc.
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            return new List<ReportModel>();
        }

        /// <summary>
        /// Creates a Report from the object passed to it and POSTs it to the database.
        /// </summary>
        /// <param name="obj">The report object to POST</param>
        public async void Create(object obj)
        {
            var reportToCreate = obj as ReportModel;
            if (reportToCreate != null)
            {
                using (Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri(_serverUrl);
                    Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    try
                    {
                        await Client.PostAsJsonAsync("reportmodels", reportToCreate);
                    }

                    catch (Exception exception)
                    {
                        throw exception;
                    }
                }
            }
            else
            {
                throw new WrongModel("The supplied model was not of the expected type");
            }
        }

        /// <summary>
        /// Deletes the specified Report from the database.
        /// </summary>
        /// <param name="obj">The report object to DELETE.</param>
        public async void Delete(object obj)
        {
            var reportToDelete = obj as ReportModel;
            if (reportToDelete != null)
            {
                using (Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri(_serverUrl);
                    try
                    {
                        await Client.DeleteAsync("reportmodels/" + reportToDelete.ReportId);
                    }

                    catch (Exception exception)
                    {
                        throw exception;
                    }
                }
            }
            else
            {
                throw new WrongModel("The supplied model was not of the expected type");
            }
        }

        /// <summary>
        /// Changes the status of the Report to Disabled.
        /// </summary>
        /// <param name="obj">The report object to disable.</param>
        public async void Disable(object obj)
        {
            var reportToDisable = obj as ReportModel;
            if (reportToDisable != null)
            {
                //Todo: Both ReportModel in solution and in Database need a ReportStatusId.
                reportToDisable.ReportStatusId = (int)ReportStatus.Disabled;
                using (Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri(_serverUrl);
                    Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    try
                    {
                        await Client.PutAsJsonAsync("reportmodels/" + reportToDisable.ReportId, reportToDisable);
                    }
                    catch (Exception exception)
                    {
                        throw exception;
                    }
                }
            }
            else
            {
                throw new WrongModel("The supplied model was not of the expected type");
            }
        }

        /// <summary>
        /// Changes the status of the Report to Suspended.
        /// </summary>
        /// <param name="obj">The report object to suspend.</param>
        public async void Suspend(object obj)
        {
            var reportToSuspend = obj as ReportModel;
            if (reportToSuspend != null)
            {
                //Todo: Both ReportModel in solution and in Database need a ReportStatusId.
                reportToSuspend.ReportStatusId = (int)ReportStatus.Suspended;
                using (Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri(_serverUrl);
                    Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    try
                    {
                        await Client.PutAsJsonAsync("reportmodels/" + reportToSuspend.ReportId, reportToSuspend);
                    }
                    catch (Exception exception)
                    {
                        throw exception;
                    }
                }
            }
            else
            {
                throw new WrongModel("The supplied model was not of the expected type");
            }
        }

        /// <summary>
        /// Updates the specified Report in the database.
        /// </summary>
        /// <param name="obj">The report object to update (PUT).</param>
        public async void Update(object obj)
        {
            var reportToUpdate = obj as ReportModel;
            if (reportToUpdate != null)
            {
                using (Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri(_serverUrl);
                    Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    try
                    {
                        await Client.PutAsJsonAsync("reportmodels/" + reportToUpdate.ReportId, reportToUpdate);
                    }

                    catch (Exception exception)
                    {
                        throw exception;
                    }
                }
            }
            else
            {
                throw new WrongModel("The supplied model was not of the expected type");
            }
        }

        /// <summary>
        /// Creates a new Report based on the parameters passed to it.
        /// </summary>
        /// <param name="target">The target profile of the report.</param>
        /// <param name="source">The profile that is making the report.</param>
        /// <param name="reportsContent">The comment provided with the report.</param>
        public ReportModel CreateNewReport(ProfileModel target, string reportsContent)
        {
            ReportModel newReport = null;
            try
            {
                newReport = new ReportModel
                {
                    //Fills in all the fields except for the ReportId
                    //Todo: Both ReportModel in solution and in Database need a ReportStatusId.
                    FK_ReportedProfile = target.ProfileId,
                    FK_ReportingProfile = ProfileHandler.GetInstance().CurrentLoggedInProfile.ProfileId,
                    ReportMessage = reportsContent,
                    ReportTime = DateTimeOffset.Now.DateTime,
                    ReportedProfile = target,
                    ReportingProfile = ProfileHandler.GetInstance().CurrentLoggedInProfile,
                    ReportStatusId = (int)ReportStatus.Active,
                };
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return newReport;
         }

        //Template for some future Exception handling

        //public class SomethingWentWrong : Exception
        //{
        //    public SomethingWentWrong()
        //    {
                
        //    }

        //    public SomethingWentWrong(string message) : base(message)
        //    {

        //    }

        //    public SomethingWentWrong(string message, Exception inner) : base(message, inner)
        //    {

        //    }
        //}
    }
}
