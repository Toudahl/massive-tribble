using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using FetchItUniversalAndApi.Annotations;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.ViewModel
{
    class NotificationHubViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<NotificationModel> _notifications;
        private NotificationModel _selectedNotification;
        private ProfileModel _fromProfile;
        private ProfileHandler _ph;

        public ObservableCollection<NotificationModel> Notifications
        {
            get { return _notifications; }
            set {_notifications = value; }
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

        public NotificationHubViewModel()
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
    }
}
