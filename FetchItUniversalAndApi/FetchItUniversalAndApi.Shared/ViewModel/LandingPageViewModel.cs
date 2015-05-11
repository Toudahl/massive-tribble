using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
        private ICommand _refreshMarketplace;
        private ObservableCollection<NotificationModel> _notifications;
        private ObservableCollection<TaskModel> _activeTasks;
        private ICommand _refreshNotifications;
        private string _currentProfileName;

        public ObservableCollection<TaskModel> Marketplace
        {
            get { return _marketplace; }
            set
            {
                _marketplace = value;
                OnPropertyChanged("Marketplace");
                //When the Marketplace is updated, it will run refreshActiveTasks() in the background
                Task updateActiveTasksTask = new Task(refreshActiveTasks);
                updateActiveTasksTask.RunSynchronously();
            }
        }

        public string CurrentProfileName
        {
            get { return _currentProfileName; }
            set { _currentProfileName = value; }
        }

        public string CurrentProfileName
        {
            get { return _currentProfileName; }
            set { _currentProfileName = value; }
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
            CurrentProfileName = ph.CurrentLoggedInProfile.ProfileName;
            Marketplace = new ObservableCollection<TaskModel>();
            ActiveTasks = new ObservableCollection<TaskModel>();
            Notifications = new ObservableCollection<NotificationModel>();
            RefreshMarketplace = new RelayCommand(refreshMarketplace);
            RefreshNotifications = new RelayCommand(refreshNotifications);
            refreshMarketplace();
            refreshNotifications();
            #region TESTING AREA! DELETE THIS SHIT!
            #region postNotification
            //NotificationModel testNotification = new NotificationModel();
            //testNotification.NotificationContent = "this is a Test";
            //testNotification.ToProfile = ph.CurrentLoggedInProfile;
            //MessageHandler.SendNotification(testNotification);
            #endregion
            #region populateListview
            //Notifications.Add(testNotification);
            //Notifications.Add(testNotification);
            //Notifications.Add(testNotification);
            #endregion
            #endregion
        }

        #endregion
        #region ICommand methods
        public async void refreshMarketplace()
        {
            //TODO: This shouldn't do anything if it's being clicked too often. But probably also good to have a cooldown on the TaskHandler method as well
            Marketplace = th.GetTasks(TaskHandler.TaskStatus.Active).ToObservableCollection();
        }

        /// <summary>
        /// Async, looks for any Tasks currently in the local Marketplace and checks if it has the current logged in profile as TaskMaster or Fetcher
        /// </summary>
        private async void refreshActiveTasks()
        {
            //TODO: This is utter shit, make the collection proper to begin with and stop shitty casting
            ActiveTasks = Marketplace.Where(t => t.MasterProfile == ph.CurrentLoggedInProfile || t.FetcherProfile == ph.CurrentLoggedInProfile).ToObservableCollection();
        }
        #region ICommand methods
        private async void refreshMarketplace()
        {
            //TODO: This shouldn't do anything if it's being clicked too often. But probably also good to have a cooldown on the TaskHandler method as well
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
