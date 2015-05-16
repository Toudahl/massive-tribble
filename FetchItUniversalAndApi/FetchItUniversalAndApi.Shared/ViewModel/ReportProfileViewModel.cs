using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using FetchItUniversalAndApi.Annotations;
using FetchItUniversalAndApi.Common;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.ViewModel
{
	// Author: Kristinn Þór Jónsson
    class ReportProfileViewModel:INotifyPropertyChanged
    {
        #region Fields
        private string _reportMessage;
        private ProfileModel _profileToReport;
        private ProfileModel _loggedInProfile;
        
        public ProfileHandler ProfileHandler { get; set; }
        public ReportHandler ReportHandler { get; set; }
        public ICommand SubmitReportCommand { get; set; }
               
        #endregion

        #region Constructor
        public ReportProfileViewModel()
        {
            ProfileHandler = ProfileHandler.GetInstance();
            ReportHandler = ReportHandler.GetInstance();
            ProfileToReport = ProfileHandler.SelectedProfile;
            LoggedInProfile = ProfileHandler.CurrentLoggedInProfile;
            SubmitReportCommand = new RelayCommand(SubmitReport);
        }
        #endregion

        #region Methods
        private void SubmitReport()
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
            
            message.ShowAsync();
        }

        public void CreateTheReport(ReportModel reportToSubmit)
        {
            try
            {
                ReportHandler.Create(reportToSubmit);
                ReportMessage = "";
            }
            catch (Exception)
            {
                throw;
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
