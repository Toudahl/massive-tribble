using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using FetchItUniversalAndApi.Annotations;
using FetchItUniversalAndApi.Common;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.ViewModel
{
	// Author: Kristinn Þór Jónsson
	class ReportProfileViewModel : INotifyPropertyChanged
	{
		#region Fields
		private string _reportMessage;
		private ProfileModel _profileToReport;
		private ProfileModel _loggedInProfile;
		private string _successMessage;

		public ProfileHandler ProfileHandler { get; set; }
		public ReportHandler ReportHandler { get; set; }
		public ICommand SubmitReportCommand { get; set; }
		public bool ReportSubmitted { get; set; }

		#endregion

		#region Constructor
		public ReportProfileViewModel()
		{
			//This success message string is binded two-way in the View, it makes a textbox
			//pop up, telling the user that reporting the profile was successful, also used 
			//to navigate back.
			SuccessMessage = "Collapsed";
			ReportSubmitted = false;

			ProfileHandler = ProfileHandler.GetInstance();
			ReportHandler = ReportHandler.GetInstance();
			ProfileToReport = ProfileHandler.SelectedProfile;
			LoggedInProfile = ProfileHandler.CurrentLoggedInProfile;
			SubmitReportCommand = new RelayCommand(SubmitReport);
		}
		#endregion

		#region Methods
		/// <summary>
		/// A method that shows the user a MessageDialog, giving him the option to submit a report he has created.
		/// </summary>
		async private void SubmitReport()
		{
			if (ReportSubmitted)
			{
				MessageDialog message = new MessageDialog("You have already reported this profile.", "Submit Report");
				await message.ShowAsync();
			}
			else
			{
				var reportToSubmit = ReportHandler.CreateNewReport(ProfileToReport, ReportMessage);
				MessageDialog message = new MessageDialog("Are you sure you want to submit this report?", "Submit Report");
				message.Commands.Add(new UICommand(
					"Yes",
					command => CreateTheReport(reportToSubmit)));

				message.Commands.Add(new UICommand(
					"No"));

				message.DefaultCommandIndex = 0;
				message.CancelCommandIndex = 1;

				await message.ShowAsync();
			}
		}

		/// <summary>
		/// Uses the Report handler to POST a report to the database.
		/// </summary>
		/// <param name="reportToSubmit"></param>
		async public void CreateTheReport(ReportModel reportToSubmit)
		{
			try
			{
				ReportHandler.Create(reportToSubmit);
				await Task.Delay(500);
				ReportMessage = "";
				SuccessMessage = "Visible";
				ReportSubmitted = true;
				await Task.Delay(5000);
				SuccessMessage = "Collapsed";
			}
			catch (Exception)
			{
				ErrorHandler.CreatingError(new ReportModel());
			}

		}
		#endregion

		#region Properties
		public ProfileModel ProfileToReport
		{
			get { return _profileToReport; }
			set
			{
				_profileToReport = value;
				OnPropertyChanged();
			}
		}

		public ProfileModel LoggedInProfile
		{
			get { return _loggedInProfile; }
			set
			{
				_loggedInProfile = value;
				OnPropertyChanged();
			}
		}

		public string ReportMessage
		{
			get { return _reportMessage; }
			set
			{
				_reportMessage = value;
				OnPropertyChanged();
			}
		}

		public string SuccessMessage
		{
			get { return _successMessage; }
			set
			{
				_successMessage = value;
				OnPropertyChanged();
			}
		}
		#endregion

		#region PropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}
