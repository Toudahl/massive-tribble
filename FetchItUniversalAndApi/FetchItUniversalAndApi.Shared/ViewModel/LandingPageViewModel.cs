using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel.Channels;
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

        public ObservableCollection<TaskModel> Marketplace
        {
            get { return _marketplace; }
            set
            {
                _marketplace = value;
                OnPropertyChanged("Marketplace");
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
            Marketplace = new ObservableCollection<TaskModel>();
            ActiveTasks = new ObservableCollection<TaskModel>();
            Notifications = new ObservableCollection<NotificationModel>();
            ph = ProfileHandler.GetInstance();
            th = TaskHandler.GetInstance();
            RefreshMarketplace = new RelayCommand(refreshMarketplace);
            RefreshNotifications = new RelayCommand(refreshNotifications);
            //TODO: Cannot do this now since TaskHandler hasn't been merged yet. The code is supposedly working according to Bruno but can't touch it atm.
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

            //refreshMarketplace();
        }

        #endregion
        public async void refreshMarketplace()
        {
            //TODO: Cannot do this now since TaskHandler hasn't been merged yet. The code is supposedly working according to Bruno but can't touch it atm.
            //TODO: This shouldn't do anything if it's being clicked too often. But probably also good to have a cooldown on the TaskHandler method as well
            Marketplace = th.GetTasks(TaskHandler.TaskStatus.Active).ToObservableCollection();
        }

        /// <summary>
        /// Asynchroneously looks for any Tasks currently in the local Marketplace and checks if it has the current logged in profile as TaskMaster or Fetcher
        /// </summary>
        private async void refreshActiveTasks()
        {
            //TODO: This is utter shit, make the collection proper to begin with and stop shitty casting
            IEnumerable<TaskModel> testTaskCollecton = Marketplace.Where(t => t.MasterProfile == ph.CurrentLoggedInProfile || t.FetcherProfile == ph.CurrentLoggedInProfile);
            foreach (TaskModel taskModel in testTaskCollecton)
            {
                if (taskModel == null)
                {
                    break;
                }
                ActiveTasks.Add(taskModel);
            }
        }

        private async void refreshNotifications()
        {
            Notifications = MessageHandler.GetNotifications().Result.ToObservableCollection();
        }
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
