using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers
{
	//Author: Kristinn Þór Jónsson
	/// <summary>
	/// Handles getting, deleting, creating and updating Reports.
	/// </summary>
    class ReportHandler : ICreate<ReportModel>, IDelete<ReportModel>, IDisable<ReportModel>, ISuspend<ReportModel>, IUpdate<ReportModel>
	{
		#region Fields, enums and Properties

		private static readonly string _serverUrl = "http://fetchit.mortentoudahl.dk/api/";
		private static Object _lockObject = new object();
		private static ReportHandler _handler;
		private static HttpClient Client { get; set; }
	    private ProfileHandler ph;
	    private ApiLink<ReportModel> apiLink;

		/// <summary>
		/// The different statuses a report can have. Values correspond to the values in the database.
		/// </summary>
		public enum ReportStatus
		{
			Active,
			Suspended,
			Disabled,
			Deleted,
		}
		#endregion

		#region Singleton and Constructor

		private ReportHandler()
		{
		    ph = ProfileHandler.GetInstance();
            apiLink = new ApiLink<ReportModel>();
		}

		/// <summary>
		/// Returns an Instance of the ReportHandler. Use this method instead of using the new keyword.
		/// </summary>
		/// <returns></returns>
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
		#endregion

		#region Methods

        #region GetReports method
        /// <summary>
		/// Gets all the report objects from the database that have the specified report status.
		/// </summary>
		/// <param name="status">The type of status to return.</param>
		/// <returns></returns>
		public async Task<IEnumerable<ReportModel>> GetReports()
		{
		    if (ph.CurrentLoggedInProfile == null) return null;
		    try
		    {
                using (var result = await apiLink.GetAsync())
                {
                    if (result != null)
                    {
                        if (result.IsSuccessStatusCode)
                        {
                            return await result.Content.ReadAsAsync<IEnumerable<ReportModel>>();
                        }
                    }
                }
            }
		    catch (Exception)
		    {
		        //TODO handle exception properly.

		        throw;
		    }
		    return null;
		}
        #endregion

        ///// <summary>
        ///// Attaches the ProfileModels corresponding to the FK_ReportedId and FK_ReportingId
        ///// </summary>
        ///// <param name="report">The report to find profiles for.</param>
        //public void FindProfiles(ReportModel report)
        //{
        //    report.FK_ReportedProfile = ph.AllProfiles.Where(profile => profile.ProfileId == report.FK_ReportedProfile).Select(profile => profile).ToList().First().ProfileId;
        //    report.FK_ReportingProfile = ph.AllProfiles.Where(profile => profile.ProfileId == report.FK_ReportingProfile).Select(profile => profile).ToList().First().ProfileId;
        //}

        #region Create method
        /// <summary>
		/// Creates a Report from the object passed to it and POSTs it to the database.
		/// </summary>
		/// <param name="report">The report object to POST</param>
        public async void Create(ReportModel report)
		{
		    if (report.FK_ReportedProfile == 0) return;
		    if (string.IsNullOrEmpty(report.ReportMessage) || string.IsNullOrWhiteSpace(report.ReportMessage)) return;
            report.FK_ReportingProfile = ph.CurrentLoggedInProfile.ProfileId; // will only work if the value was 0
		    report.ReportTime = DateTime.UtcNow;
		    try
		    {
                using (var result = await apiLink.PostAsJsonAsync(report))
                {
                    if (result == null) return;
                    if (result.IsSuccessStatusCode)
                    {
                        //TODO notify of success
                    }
                }
            }
		    catch (Exception)
		    {
		        //TODO handle exception
		        throw;
		    }

		}
        #endregion

        /// <summary>
		/// Currently does NOTHING. There are no report status in db.
		/// Deletes the specified Report from the database.
		/// </summary>
		/// <param name="obj">The report object to DELETE.</param>
        public async void Delete(ReportModel reportToDelete)
        {
            //TODO: Make some proper checks
            //var reportToDelete = obj as ReportModel;
            //if (reportToDelete == null)
            //{
            //    ErrorHandler.WrongModelError(obj, new ReportModel());
            //}
                //using (Client = new HttpClient())
                //{
                //    Client.BaseAddress = new Uri(_serverUrl);
                //    try
                //    {
                //        await Client.DeleteAsync("reportmodels/" + reportToDelete.ReportId);
                //    }

                //    catch (Exception)
                //    {
                //        ErrorHandler.DeletingError(new ReportModel());
                //    }
                //}
		}

	    /// <summary>
        /// Currently does NOTHING. There are no report status in db.
        /// Changes the status of the Report to Disabled.
		/// </summary>
		/// <param name="obj">The report object to disable.</param>
        public void Disable(ReportModel reportToDisable)
	    {
            ////var reportToDisable = obj as ReportModel;
            ////if (reportToDisable == null)
            ////{
            ////    ErrorHandler.WrongModelError(obj, new ReportModel());
            ////}
            //    //Todo: Both ReportModel in solution and in Database need a ReportStatusId.
            //    //reportToDisable.ReportStatusId = (int)ReportStatus.Disabled;
            //    try
            //    {
            //        Update(reportToDisable);
            //    }
            //    catch (Exception)
            //    {
            //        ErrorHandler.DisablingError(new ReportModel());
            //    }
	    }

	    /// <summary>
        /// Currently does NOTHING. There are no report status in db.
        /// Changes the status of the Report to Suspended.
		/// </summary>
		/// <param name="obj">The report object to suspend.</param>
        public void Suspend(ReportModel reportToSuspend)
	    {
            ////TODO: Make some proper checks
            ////var reportToSuspend = obj as ReportModel;
            ////if (reportToSuspend == null)
            ////{
            ////    ErrorHandler.WrongModelError(obj, new ReportModel());
            ////}
            //    //Todo: Both ReportModel in solution and in Database need a ReportStatusId.
            //    //reportToSuspend.ReportStatusId = (int)ReportStatus.Suspended;
            //    try
            //    {
            //        Update(reportToSuspend);
            //    }
            //    catch (Exception)
            //    {
            //        ErrorHandler.SuspendingError(new ReportModel());
            //    }
	    }

        #region Update method
        /// <summary>
	    /// Updates the specified Report in the database.
	    /// </summary>
        /// <param name="reportToUpdate">The report object to update (PUT).</param>
	    public async void Update(ReportModel reportToUpdate)
	    {
	        if (ph.CurrentLoggedInProfile == null) return;
	        try
	        {
	            using (var result = await apiLink.PutAsJsonAsync(reportToUpdate, reportToUpdate.ReportId))
	            {
	                if (result == null) return;
	                if (result.IsSuccessStatusCode)
	                {
	                    //TODO indicate success.
	                }
	            }
	        }
	        catch (Exception)
	        {
	            //TODO handle exception
	            throw;
	        }
	    }
        #endregion

        /// <summary>
		/// Creates a new Report based on the parameters passed to it.
		/// </summary>
		/// <param name="target">The target profile of the report.</param>
		/// <param name="source">The profile that is making the report.</param>
		/// <param name="reportsContent">The comment provided with the report.</param>
		public ReportModel CreateNewReport(ProfileModel target, string reportsContent)
		{
            return new ReportModel
            {
                //Fills in all the fields except for the ReportId
                FK_ReportedProfile = target.ProfileId,
                FK_ReportingProfile = ph.CurrentLoggedInProfile.ProfileId,
                ReportMessage = reportsContent,
                ReportTime = DateTime.UtcNow,
            };
		}
		#endregion
	}
}
