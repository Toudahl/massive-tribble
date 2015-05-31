using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
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
        //The collections being presented by the view
        private ObservableCollection<TaskModel> _marketplace;
        private ObservableCollection<NotificationModel> _notifications;
        private ObservableCollection<TaskModel> _activeTasks;
        //Commands being bound to buttons in the view
        private ICommand _refreshNotifications;
        private ICommand _refreshMarketplace;
        //The text that is instanciated in constructor, welcomes the user with his name
        private string _welcomeText;

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

        //This is contains all active tasks, where users look for new tasks
        public ObservableCollection<TaskModel> Marketplace
        {
            get { return _marketplace; }
            set
            {
                if (value != null)
                {
                    _marketplace = value;
                    OnPropertyChanged();
                    //When the Marketplace is updated, it will run refreshActiveTasks() in the background
                    Task updateActiveTasksTask = new Task(refreshActiveTasks);
                    updateActiveTasksTask.RunSynchronously();
                }
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

        //Where the user is presented with all tasks that he is assigned to
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

        public string WelcomeText
        {
            get { return _welcomeText; }
            set
            {
                _welcomeText = value;
                OnPropertyChanged("WelcomeText");
            }
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
        /// Looks for any Tasks currently in the local Marketplace and checks if it has the current logged in profile as TaskMaster or Fetcher
        /// </summary>
        private void refreshActiveTasks()
        {
            ActiveTasks = th.GetTasks(TaskHandler.TaskStatus.All).Where(p => p.FK_TaskFetcher == ph.CurrentLoggedInProfile.ProfileId || p.FK_TaskMaster == ph.CurrentLoggedInProfile.ProfileId).ToObservableCollection();
        }

        #region ICommand methods
        /// <summary>
        /// Gets active tasks, called upon in the constructor and used by 2 buttons in view.
        /// 1 button next to the marketplace and another next to 'Your Tasks'
        /// That's because 'Your Tasks' list is refreshed when the Marketplace property is set
        /// </summary>
        private void refreshMarketplace()
        {
            Marketplace = th.GetTasks(TaskHandler.TaskStatus.Active).ToObservableCollection();
        }

        /// <summary>
        /// Gets the notifications associated with the currently logged in profile
        /// </summary>
        private void refreshNotifications()
        {
            Notifications = MessageHandler.GetNotifications().ToObservableCollection();
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
