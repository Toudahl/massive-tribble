using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Windows.UI.Popups;
using FetchItUniversalAndApi.Annotations;
using FetchItUniversalAndApi.Common;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.ViewModel
{
    class CreateFeedbackViewModel:INotifyPropertyChanged
    {
        #region Fields
        private string _optionalText;
        private int _rating;

        public TaskHandler TaskHandler { get; set; }
        public TaskModel SelectedTask { get; set; }
        public ProfileHandler ProfileHandler { get; set; }
        public ProfileModel LoggedInProfile { get; set; }
        public ICommand SubmitFeedbackCommand { get; set; }
        #endregion

        #region Constructor
        public CreateFeedbackViewModel()
        {
            TaskHandler = TaskHandler.GetInstance();
            ProfileHandler = ProfileHandler.GetInstance();
            LoggedInProfile = ProfileHandler.CurrentLoggedInProfile;
            SelectedTask = TaskHandler.SelectedTask;

            SubmitFeedbackCommand = new RelayCommand(SubmitFeedback);
            
        }
        #endregion

        #region Methods
        private void SubmitFeedback()
        {
            MessageDialog message = new MessageDialog("Are you sure you want to submit this feedback?", "Submit Feedback");
            message.Commands.Add(new UICommand(
                "Yes",
                command => CreateTheFeedback()));
            
            message.Commands.Add(new UICommand(
                "No"));

            message.DefaultCommandIndex = 0;
            message.CancelCommandIndex = 1;

            message.ShowAsync();
        }
        
        public void CreateTheFeedback()
        {
            try
            {
                MessageHandler.CreateFeedback(Rating, OptionalText, SelectedTask);
                OptionalText = "";
                Rating = 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Properties
        public string OptionalText
        {
            get { return _optionalText; }
            set
            {
                _optionalText = value; 
                OnPropertyChanged();
            }
        }
        public int Rating
        {
            get { return _rating; }
            set
            {
                _rating = value; 
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
