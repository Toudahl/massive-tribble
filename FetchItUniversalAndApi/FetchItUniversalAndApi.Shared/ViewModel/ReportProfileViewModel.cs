using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            MessageDialog message2 = new MessageDialog("HOHOHO");
            message.Commands.Add(new UICommand(
                "Yes",
                command => ReportHandler.Create(reportToSubmit)));
            message.Commands.Add(new UICommand(
                "No", async command => await message2.ShowAsync()));
                
            
            message.DefaultCommandIndex = 0;
            message.CancelCommandIndex = 1;

            message.ShowAsync();
        }
        #endregion

        #region Proferties
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
