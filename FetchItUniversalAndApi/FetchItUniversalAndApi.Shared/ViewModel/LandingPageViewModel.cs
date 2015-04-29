using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Windows.Foundation.Collections;
using FetchItUniversalAndApi.Common;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.ViewModel
{
    //Author: Lárus Þór Awesomeson
    class LandingPageViewModel
    {
        #region Fields and Properties
        private ProfileHandler _ph;
        private TaskHandler _th;
        private ObservableCollection<TaskModel> _marketplace;
        private ICommand _refreshMarketplace;
        private ObservableCollection<NotificationModel> _notifications;
        private ObservableCollection<TaskModel> _activeTasks;

        public ObservableCollection<TaskModel> Marketplace
        {
            get { return _marketplace; }
            set { _marketplace = value; }
        }

        public ObservableCollection<NotificationModel> Notifications
        {
            get { return _notifications; }
            set { _notifications = value; }
        }

        public ObservableCollection<TaskModel> ActiveTasks
        {
            get { return _activeTasks; }
            set { _activeTasks = value; }
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
            //TODO: Cannot do this now since TaskHandler hasn't been merged yet. The code is supposedly working according to Bruno but can't touch it atm.
            #region TESTING AREA! DELETE THIS SHIT!
            NotificationModel testNotification = new NotificationModel();
            testNotification.NotificationContent = "this is a Test";
            testNotification.ToProfile = ph.CurrentLoggedInProfile;
            MessageHandler.SendNotification(testNotification);
            #endregion   
            //refreshMarketplace();
            refreshNotifications();
        }

        #endregion
        public async void refreshMarketplace()
        {
            //TODO: Cannot do this now since TaskHandler hasn't been merged yet. The code is supposedly working according to Bruno but can't touch it atm.
            //TODO: This shouldn't do anything if it's being clicked too often. But probably also good to have a cooldown on the TaskHandler method as well
            //Marketplace = th.GetTasks();
            throw new NotImplementedException("Need to merge Taskhandler to gain access to Taskhandler.GetTasks()");
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
            IEnumerable<NotificationModel> testNotificationCollecton = MessageHandler.GetNotifications();
            foreach (NotificationModel notification in testNotificationCollecton)
            {
                if (notification == null)
                {
                    break;
                }
                notification.ToProfile = ph.CurrentLoggedInProfile;
                Notifications.Add(notification);
            }
        }
        #endregion
    }
}
