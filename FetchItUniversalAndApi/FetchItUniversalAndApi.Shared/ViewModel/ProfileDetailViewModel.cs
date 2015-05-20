using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using FetchItUniversalAndApi.Annotations;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;


namespace FetchItUniversalAndApi.ViewModel
{
    class ProfileDetailViewModel:INotifyPropertyChanged
    {
        #region properties
        public ProfileHandler ProfileHandler { get; set; }
        public string selectedprofileName { get; set; }
        public string selectedprofileMobile { get; set; }
        public string selectedprofileAddress { get; set; }
        public string selectedprofileEmail { get; set; }
        public int selectedprofileId { get; set; }
        public string selectedProfileRating { get; set; }
        public string selectedProfileDescription { get; set; }
        public string IsProfileVerified { get; set; }
        public TaskHandler TaskHandler { get; set; }
        public IEnumerable<TaskModel> AllTasks { get; set; }
        #endregion



        public ProfileDetailViewModel()
        {
            ProfileHandler = ProfileHandler.GetInstance();
           TaskHandler  = TaskHandler.GetInstance();


            selectedprofileName = ProfileHandler.CurrentLoggedInProfile.ProfileName;
            selectedprofileMobile = ProfileHandler.CurrentLoggedInProfile.ProfileMobile;
            selectedprofileEmail = ProfileHandler.CurrentLoggedInProfile.ProfileEmail;
            selectedprofileAddress = ProfileHandler.CurrentLoggedInProfile.ProfileAddress;
            selectedprofileId = ProfileHandler.CurrentLoggedInProfile.ProfileId;
            selectedProfileDescription = ProfileHandler.CurrentLoggedInProfile.ProfileText;
            IsProfileVerified = ProfileHandler.CurrentLoggedInProfile.ProfileIsVerified==false ? "No" : "Yup";
            selectedProfileRating ="Rating: "+ GetAverageRating();










        }

        public IEnumerable<TaskModel> GetCurrentUsersTasks()
        {
            return
                TaskHandler.GetTasks(TaskHandler.TaskStatus.Active)
                .Where(task => task.FK_TaskFetcher == ProfileHandler.CurrentLoggedInProfile.ProfileId);
        }

        public double GetAverageRating()
        {
            
            
                
            
            var allfedbacks=MessageHandler.GetFeedback(MessageHandler.FeedbackStatus.Active);

            var UsersFeedbacks = from task in GetCurrentUsersTasks()
                join feedback in allfedbacks
                    on task.TaskId equals feedback.FK_FeedbackForTask
                select new
                {
                    Rating = feedback.FeedbackRating
                };
           
           
            try
            {
              return UsersFeedbacks.Average(f => f.Rating);
            }
            catch (Exception)
            {

                return 0;
            }
        }
        
        #region prop changed
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}