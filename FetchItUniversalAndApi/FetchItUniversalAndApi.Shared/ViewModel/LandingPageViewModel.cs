using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation.Collections;
using FetchItUniversalAndApi.Annotations;
using FetchItUniversalAndApi.Common;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.ViewModel
{
    //Author: Lárus Þór Awesomeson
    class LandingPageViewModel : INotifyPropertyChanged
    {
        #region Fields and Properties
        private ProfileHandler _ph;
        private TaskHandler _th;
        private ObservableCollection<TaskModel> _marketplace;
        private ObservableCollection<NotificationModel> _notifications;
        private ObservableCollection<TaskModel> _activeTasks;
        private ICommand _refreshNotifications;
        private ICommand _refreshMarketplace;
        private string _welcomeText;

        public ObservableCollection<TaskModel> Marketplace
        {
            get { return _marketplace; }
            set
            {
                if (value != null)
                {
                    _marketplace = value;
                    OnPropertyChanged("Marketplace");
                    //When the Marketplace is updated, it will run refreshActiveTasks() in the background
                    Task updateActiveTasksTask = new Task(refreshActiveTasks);
                    updateActiveTasksTask.RunSynchronously();
                }
            }
        }

        public ProfileHandler ph
        {
            get { return _ph; }
            set { _ph = value; }
        }

        public TaskHandler th
        {
            get { return _th; }
            set { _th = value; }
        }

        public string WelcomeText
        {
            get { return _welcomeText; }
            set
            {
                _welcomeText = value;
                OnPropertyChanged("WelcomeText");
            }
        }

        public ObservableCollection<NotificationModel> Notifications
        {
            get { return _notifications; }
            set
            {
                _notifications = value;
                OnPropertyChanged("Notifications");
            }
        }

        public ObservableCollection<TaskModel> ActiveTasks
        {
            get { return _activeTasks; }
            set
            {
                _activeTasks = value;
                OnPropertyChanged("ActiveTasks");
            }
        }

        public ICommand RefreshMarketplace
        {
            get { return _refreshMarketplace; }
            set { _refreshMarketplace = value; }
        }

        public ICommand RefreshNotifications
        {
            get { return _refreshNotifications; }
            set { _refreshNotifications = value; }
        }
        #endregion

        #region Methods
        #region Constructor
        public LandingPageViewModel()
        {
            ph = ProfileHandler.GetInstance();
            th = TaskHandler.GetInstance();
            Marketplace = new ObservableCollection<TaskModel>();
            ActiveTasks = new ObservableCollection<TaskModel>();
            Notifications = new ObservableCollection<NotificationModel>();
            RefreshMarketplace = new RelayCommand(refreshMarketplace);
            RefreshNotifications = new RelayCommand(refreshNotifications);
            WelcomeText = "Welcome " + ph.CurrentLoggedInProfile.ProfileName + "!";
            refreshMarketplace();
            refreshNotifications();
        }
        #endregion

        /// <summary>
        /// Async, looks for any Tasks currently in the local Marketplace and checks if it has the current logged in profile as TaskMaster or Fetcher
        /// </summary>
        private async void refreshActiveTasks()
        {
            ActiveTasks = Marketplace.Where(t => t.MasterProfile == ph.CurrentLoggedInProfile || t.FetcherProfile == ph.CurrentLoggedInProfile).ToObservableCollection();
        }

        #region ICommand methods
        private async void refreshMarketplace()
        {
            Marketplace = th.GetTasks(TaskHandler.TaskStatus.Active).ToObservableCollection();
        }

        /// <summary>
        /// Async, gets the notifications associated with the currently logged in profile
        /// </summary>
        private async void refreshNotifications()
        {
            Notifications = MessageHandler.GetNotifications().Result.ToObservableCollection();
        }
        #endregion
        #endregion

        #region INotifyPropertyChanged
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
