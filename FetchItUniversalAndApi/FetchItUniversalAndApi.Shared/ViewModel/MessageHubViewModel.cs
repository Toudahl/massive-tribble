using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FetchItUniversalAndApi.Annotations;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.ViewModel
{
    class MessageHubViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<NotificationModel> _notifications;
        private NotificationModel _selectedNotification;
        private ProfileModel _fromProfile;
        private ProfileHandler _ph;
        private ObservableCollection<FeedbackModel> _feedbacks;

        public ObservableCollection<NotificationModel> Notifications
        {
            get { return _notifications; }
            set {_notifications = value; }
        }

        public ObservableCollection<FeedbackModel> Feedbacks
        {
            get { return _feedbacks; }
            set { _feedbacks = value; }
        }

        public ProfileHandler ph
        {
            get { return _ph; }
            set { _ph = value; }
        }

        public TaskHandler th { get; set; }

        public NotificationModel SelectedNotification
        {
            get { return _selectedNotification; }
            set
            {
                _selectedNotification = value;
                
                FromProfile = ph.AllProfiles.First(n => n.ProfileId == SelectedNotification.FK_NotificationFrom);
                OnPropertyChanged("SelectedNotification");
            }
        }

        public MessageHubViewModel()
        {
            ph = ProfileHandler.GetInstance();
            th = TaskHandler.GetInstance();
            ph.GetAllProfiles();
            GetNotifications();
        }

        public void GetNotifications()
        {
            Notifications = MessageHandler.GetNotifications().Result.ToObservableCollection();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }


        public ProfileModel FromProfile
        {
            get { return _fromProfile; }
            set
            {
                _fromProfile = value;
                OnPropertyChanged("FromProfile");
            }
        }

        public void GetFeedback(MessageHandler.FeedbackStatus status, ProfileModel feedbackProfile)
        {
            try
            {
                using (HttpClient msgWebClient = new HttpClient())
                {
                    var tasks = TaskHandler.GetInstance().GetTasks(TaskHandler.TaskStatus.Completed);
                    var reportsStream = Task.Run(async () => await msgWebClient.GetAsync("FeedbackModels"));
                    var feedbacks = reportsStream.Result.Content.ReadAsAsync<IEnumerable<FeedbackModel>>().Result;
                    var returnedFeedbacks = from task in tasks
                                            join feedback in feedbacks
                                                on task.TaskId equals feedback.FK_FeedbackForTask
                                            where
                                                task.FK_TaskFetcher == feedbackProfile.ProfileId ||
                                                task.FK_TaskMaster == feedbackProfile.ProfileId
                                            select feedback;
                    ;
                    Feedbacks = returnedFeedbacks.ToObservableCollection();
                }
               
            }
            catch (Exception exception)
            {
                //Add standardized error handling (fx. LogHandler.GetInstance().LogEvent(exception.message) and MessageBox.Show("Yo user, something went wrong!"));
                throw exception;
            }
        }
    }
}
