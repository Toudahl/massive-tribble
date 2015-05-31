using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FetchItUniversalAndApi.Annotations;
using FetchItUniversalAndApi.Common;
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
        private RelayCommand _refreshFeedback;
        private RelayCommand _refreshNotifications;
        private FeedbackModel _selectedFeedback;

        #region Properties
        public ObservableCollection<NotificationModel> Notifications
        {
            get { return _notifications; }
            set {_notifications = value; }
        }

        public ObservableCollection<FeedbackModel> Feedbacks
        {
            get { return _feedbacks; }
            set
            {
                _feedbacks = value; 
                OnPropertyChanged("Feedbacks");
            }
        }

        public ProfileHandler ph
        {
            get { return _ph; }
            set { _ph = value; }
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

        public RelayCommand RefreshNotifications
        {
            get { return _refreshNotifications; }
            set { _refreshNotifications = value; }
        }

        public RelayCommand RefreshFeedback
        {
            get { return _refreshFeedback; }
            set { _refreshFeedback = value; }
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

        public FeedbackModel SelectedFeedback
        {
            get { return _selectedFeedback; }
            set
            {
                _selectedFeedback = value;
                OnPropertyChanged("SelectedFeedback");
                GetFeedbackTask();
            }
        }

        #endregion


        public MessageHubViewModel()
        {
            ph = ProfileHandler.GetInstance();
            th = TaskHandler.GetInstance();
            GetNotifications();
            GetFeedback();
            RefreshNotifications = new RelayCommand(GetNotifications);
            RefreshFeedback = new RelayCommand(GetFeedback);
        }


        #region Methods

        public void GetNotifications()
        {
            Notifications = MessageHandler.GetNotifications().ToObservableCollection();
        }

        public void GetFeedback()
        {
            Feedbacks = MessageHandler.GetFeedback(MessageHandler.FeedbackStatus.Active, ph.CurrentLoggedInProfile).ToObservableCollection();
        }

        public void GetFeedbackTask()
        {
            TaskModel tm = new TaskModel();
            tm.TaskId = SelectedFeedback.FK_FeedbackForTask;
            IEnumerable<TaskModel> feedbackTask = (IEnumerable<TaskModel>) th.Search(tm);

            ProfileModel pm = new ProfileModel();
            pm.ProfileId = feedbackTask.First().FK_TaskMaster;
            IEnumerable<ProfileModel> feedbackProfile = (IEnumerable<ProfileModel>) ph.Search(pm);
            FromProfile = feedbackProfile.First();
        }



        #endregion

        #region INotifyPropertychanged

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
