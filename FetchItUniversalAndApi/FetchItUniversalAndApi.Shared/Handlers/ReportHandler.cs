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
	class ReportHandler : ICreate, IDelete, IDisable, ISuspend, IUpdate
	{
		#region Fields, enums and Properties

		private static readonly string _serverUrl = "http://fetchit.mortentoudahl.dk/api/";
		private static Object _lockObject = new object();
		private static ReportHandler _handler;
		private static HttpClient Client { get; set; }

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
					var reportsToReturn = reports.Result.Content.ReadAsAsync<IEnumerable<ReportModel>>().Result;

					//Attaches the Reporting and Reported Profile models (Navigational properties)
					foreach (var reportModel in reportsToReturn)
					{
						FindProfiles(reportModel);
					}
					return reportsToReturn;

					//TODO: Possibly find another way to use the Report Status to get the specified reports.
					//Through something called Attribute routing, it is easily possible to make the 
					//url "reportmodels/status/1" return all statuses numbered 1, etc.
				}
				catch (Exception)
				{
					ErrorHandler.GettingError(new ReportModel());
				}
			}
			return new List<ReportModel>();
		}

		/// <summary>
		/// Attaches the ProfileModels corresponding to the FK_ReportedId and FK_ReportingId
		/// </summary>
		/// <param name="report">The report to find profiles for.</param>
		public void FindProfiles(ReportModel report)
		{
			var ph = ProfileHandler.GetInstance();
			report.ReportedProfile = ph.AllProfiles.Where(profile => profile.ProfileId == report.FK_ReportedProfile).Select(profile => profile).ToList().First();
			report.ReportingProfile = ph.AllProfiles.Where(profile => profile.ProfileId == report.FK_ReportingProfile).Select(profile => profile).ToList().First();
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
				//Sets the navigational properties to null, to make sure that the report can POSTed
				reportToCreate.ReportedProfile = null;
				reportToCreate.ReportingProfile = null;
				using (Client = new HttpClient())
				{
					Client.BaseAddress = new Uri(_serverUrl);
					Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					try
					{
						await Client.PostAsJsonAsync("reportmodels", reportToCreate);

						//Attaches the navigational properties again to the reportmodel.
						FindProfiles(reportToCreate);
					}

					catch (Exception)
					{
						ErrorHandler.CreatingError(new ReportModel());

						//Attaches the navigational properties again to the reportmodel.
						FindProfiles(reportToCreate);
					}
				}
			}
			else
			{
				ErrorHandler.WrongModelError(obj, new ReportModel());
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

					catch (Exception)
					{
						ErrorHandler.DeletingError(new ReportModel());
					}
				}
			}
			else
			{
				ErrorHandler.WrongModelError(obj, new ReportModel());
			}
		}

		/// <summary>
		/// Changes the status of the Report to Disabled.
		/// </summary>
		/// <param name="obj">The report object to disable.</param>
		public void Disable(object obj)
		{
			var reportToDisable = obj as ReportModel;
			if (reportToDisable != null)
			{
				//Todo: Both ReportModel in solution and in Database need a ReportStatusId.
				//reportToDisable.ReportStatusId = (int)ReportStatus.Disabled;
				try
				{
					Update(reportToDisable);
				}
				catch (Exception)
				{
					ErrorHandler.DisablingError(new ReportModel());
				}
			}
			else
			{
				ErrorHandler.WrongModelError(obj, new ReportModel());
			}
		}

		/// <summary>
		/// Changes the status of the Report to Suspended.
		/// </summary>
		/// <param name="obj">The report object to suspend.</param>
		public void Suspend(object obj)
		{
			var reportToSuspend = obj as ReportModel;
			if (reportToSuspend != null)
			{
				//Todo: Both ReportModel in solution and in Database need a ReportStatusId.
				//reportToSuspend.ReportStatusId = (int)ReportStatus.Suspended;
				try
				{
					Update(reportToSuspend);
				}
				catch (Exception)
				{
					ErrorHandler.SuspendingError(new ReportModel());
				}
			}
			else
			{
				ErrorHandler.WrongModelError(obj, new ReportModel());
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
				reportToUpdate.ReportedProfile = null;
				reportToUpdate.ReportingProfile = null;
				using (Client = new HttpClient())
				{
					Client.BaseAddress = new Uri(_serverUrl);
					Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					try
					{
						await Client.PutAsJsonAsync("reportmodels/" + reportToUpdate.ReportId, reportToUpdate);
						FindProfiles(reportToUpdate);
					}

					catch (Exception)
					{
						ErrorHandler.UpdatingError(new ReportModel());
						FindProfiles(reportToUpdate);
					}
				}
			}
			else
			{
				ErrorHandler.WrongModelError(obj, new ReportModel());
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
					ReportTime = DateTime.UtcNow,

					//Fills in the navigational properties, it is working because all the methods
					//in ReportHandler involving POST and PUT have working checks.
					ReportedProfile = target,
					ReportingProfile = ProfileHandler.GetInstance().CurrentLoggedInProfile,
					//ReportStatusId = (int)ReportStatus.Active,
				};
			}
			catch (Exception exception)
			{
				ErrorHandler.DisplayErrorMessage("Creating the report object failed", "Contact support");
			}
			return newReport;
		}
		#endregion
	}
}
