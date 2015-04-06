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
    class ReportHandler: ICreate, IDelete, IDisable, ISuspend, IUpdate
    {
        //TODO: Might need to change the Url.
        private static readonly string _serverUrl = "http://localhost:36904/api/";
        private static Object _lockObject = new object();
        private static ReportHandler _handler;
        private static HttpClient Client { get; set; }

        // Must be set to the same values as the values in the db.
        public enum ReportStatus
        {

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
        public async Task<IEnumerable<ReportModel>> GetReports(ReportStatus status)
        {
            using (Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(_serverUrl);
                try
                {
                    //It is possible to make the url "reports/1" return all statuses numbered 1, etc.
                    //TODO: Possibly find another way to use the Report Status to get the specified reports.
                    var reportsToReturn = await Client.GetAsync("reports/" + (int)status);
                    if (reportsToReturn.IsSuccessStatusCode)
                    {
                        var results = reportsToReturn.Content.ReadAsAsync<IEnumerable<ReportModel>>().Result;
                        return results.ToList();
                    }
                }
                catch (Exception exception)
                {
                    new MessageDialog(exception.Message).ShowAsync();
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
                        await Client.PostAsJsonAsync("reports", reportToCreate);
                    }

                    //TODO: Create better exception handling.
                    catch (Exception exception)
                    {
                        new MessageDialog(exception.Message).ShowAsync();
                    }
                }
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
                        await Client.DeleteAsync("reports/" + reportToDelete.ReportId);
                    }

                    //TODO: Create better exception handling.
                    catch (Exception exception)
                    {
                        new MessageDialog(exception.Message).ShowAsync();
                    }
                }
            }
        }

        public void Disable(object obj)
        {
            throw new NotImplementedException();
        }

        public void Suspend(object obj)
        {
            throw new NotImplementedException();
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
                        await Client.PutAsJsonAsync("reports/" + reportToUpdate.ReportId, reportToUpdate);
                    }

                    //TODO: Create better exception handling.
                    catch (Exception exception)
                    {
                        new MessageDialog(exception.Message).ShowAsync();
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new Report based on the parameters passed to it.
        /// </summary>
        /// <param name="target">The target profile of the report.</param>
        /// <param name="source">The profile that is making the report.</param>
        /// <param name="reportsContent">The comment provided with the report.</param>
        public ReportModel CreateNewReport(ProfileModel target, ProfileModel source, string reportsContent)
        {
            ReportModel newReport = null;
            try
            {
                newReport = new ReportModel
                {
                    //Fills in all the fields except for the ReportId
                    FK_ReportedProfile = target.ProfileId,
                    FK_ReportingProfile = source.ProfileId,
                    ReportMessage = reportsContent,
                    ReportTime = DateTimeOffset.Now.DateTime,
                    ReportedProfile = target,
                    ReportingProfile = source
                };
            }
            catch (Exception exception)
            {
                new MessageDialog(exception.Message).ShowAsync();
            }
            return newReport;
         }
    }
}
