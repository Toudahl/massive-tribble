using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Windows.UI.Popups;
using FetchItUniversalAndApi.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;
using FetchItUniversalAndApi.ViewModel;

namespace FetchItUniversalAndApi.View
{
	/// <summary>
	/// A basic page that provides characteristics common to most applications.
	/// </summary>
	public sealed partial class TaskDetailPage : Page
	{
		private NavigationHelper navigationHelper;
		private ObservableDictionary defaultViewModel = new ObservableDictionary();
	    private ProfileHandler ph;

		/// <summary>
		/// This can be changed to a strongly typed view model.
		/// </summary>
		public ObservableDictionary DefaultViewModel
		{
			get { return this.defaultViewModel; }
		}

		/// <summary>
		/// NavigationHelper is used on each page to aid in navigation and 
		/// process lifetime management
		/// </summary>
		public NavigationHelper NavigationHelper
		{
			get { return this.navigationHelper; }
		}


		public TaskDetailPage()
		{
			this.InitializeComponent();
			this.navigationHelper = new NavigationHelper(this);
			this.navigationHelper.LoadState += navigationHelper_LoadState;
			this.navigationHelper.SaveState += navigationHelper_SaveState;
            ph = ProfileHandler.GetInstance();
		    ph.LogOutEvent += NavigateToLogIn;
		}

		/// <summary>
		/// Populates the page with content passed during navigation. Any saved state is also
		/// provided when recreating a page from a prior session.
		/// </summary>
		/// <param name="sender">
		/// The source of the event; typically <see cref="NavigationHelper"/>
		/// </param>
		/// <param name="e">Event data that provides both the navigation parameter passed to
		/// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
		/// a dictionary of state preserved by this page during an earlier
		/// session. The state will be null the first time a page is visited.</param>
		private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
		{
		}

		/// <summary>
		/// Preserves state associated with this page in case the application is suspended or the
		/// page is discarded from the navigation cache.  Values must conform to the serialization
		/// requirements of <see cref="SuspensionManager.SessionState"/>.
		/// </summary>
		/// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
		/// <param name="e">Event data that provides an empty dictionary to be populated with
		/// serializable state.</param>
		private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
		{
		}

		#region NavigationHelper registration

		/// The methods provided in this section are simply used to allow
		/// NavigationHelper to respond to the page's navigation methods.
		/// 
		/// Page specific logic should be placed in event handlers for the  
		/// <see cref="Common.NavigationHelper.LoadState"/>
		/// and <see cref="Common.NavigationHelper.SaveState"/>.
		/// The navigation parameter is available in the LoadState method 
		/// in addition to page state preserved during an earlier session.

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedTo(e);

			RefreshButtons();
		}

		//This method gets called everytime the buttons visibility need to be refreshed
		private void RefreshButtons()
		{
			var th = TaskHandler.GetInstance();
			var ph = ProfileHandler.GetInstance();

			CreateFeedbackButton.Visibility = Visibility.Collapsed;
			EditTaskButton.Visibility = Visibility.Collapsed;
			AssignToTaskButton.Visibility = Visibility.Collapsed;
			ResignFromTaskButton.Visibility = Visibility.Collapsed;
			MarkAsCompletedButton.Visibility = Visibility.Collapsed;

			#region RefreshingButtons
			//This code makes the Create Feedback button visible on three conditions:
			//1. If the current logged in profile is the taskmaster of the task.
			//2. If the task does not have any feedbacks.
			//3. If the task status is set to 5 (or TaskStatus.Completed)

			if (th.SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.Completed)
			{
				if (th.SelectedTask.Feedbacks.Count < 1 && ph.CurrentLoggedInProfile.ProfileId == th.SelectedTask.FK_TaskMaster)
				{
					CreateFeedbackButton.Visibility = Visibility.Visible;
				}
			}
			else
			{

				//This code makes the EditTask button visible if:
				//1. Loggedin profile is the Taskmaster
				//2. TaskStatus is active or reported
				if (th.SelectedTask.FK_TaskMaster == ph.CurrentLoggedInProfile.ProfileId)
				{
					if (th.SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.Active || th.SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.Reported)
					{
						EditTaskButton.Visibility = Visibility.Visible;
					}
				}
			}

			//This code makes the AssignToTask button visible if:
			//1. The task has no fetcher
			//2. The current logged in profile is not the taskmaster
			//3. The status of the task is active
			if (th.SelectedTask.FK_TaskFetcher == null && th.SelectedTask.FK_TaskMaster != ph.CurrentLoggedInProfile.ProfileId)
			{
				if (th.SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.Active)
				{
					AssignToTaskButton.Visibility = Visibility.Visible;
				}
			}

			//This codes makes the Resign button visible if:
			//1. The current logged in profile is a fetcher for the task.
			//2. The task status is active
			if (th.SelectedTask.FK_TaskFetcher == ph.CurrentLoggedInProfile.ProfileId)
			{
				if (th.SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.Active)
				{
					ResignFromTaskButton.Visibility = Visibility.Visible;
				}
			}

			//This code makes the MarkAsCompleted button visible if:
			//1. The current logged in profile is either a fethcer or a taskmaster
			//2. The task status is either: Active, FetcherCompleted or TaskMasterCompleted
			//3. The task already has a fethcer assigned to it
			if (th.SelectedTask.FK_TaskFetcher == ph.CurrentLoggedInProfile.ProfileId ||
				th.SelectedTask.FK_TaskMaster == ph.CurrentLoggedInProfile.ProfileId)
			{

				if (th.SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.Active ||
					th.SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.FetcherCompleted ||
					th.SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.TaskMasterCompleted)
				{
					if (th.SelectedTask.FK_TaskFetcher != null)
					{
						MarkAsCompletedButton.Visibility = Visibility.Visible;
					}


					//This just makes sure that if a Master or Fethcer has marked it as completed, that
					//he does not see the button pop up again
					if (th.SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.FetcherCompleted &&
						th.SelectedTask.FK_TaskFetcher == ph.CurrentLoggedInProfile.ProfileId)
					{
						MarkAsCompletedButton.Visibility = Visibility.Collapsed;
					}

					else if (th.SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.TaskMasterCompleted &&
						th.SelectedTask.FK_TaskMaster == ph.CurrentLoggedInProfile.ProfileId)
					{
						MarkAsCompletedButton.Visibility = Visibility.Collapsed;
					}
				}
			}
			#endregion
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedFrom(e);
		}

		#endregion

		private void CreateFeedbackButton_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(CreateFeedbackPage));
		}

		private void EditTaskButton_Click(object sender, RoutedEventArgs e)
		{
			var th = TaskHandler.GetInstance();
			if (th.SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.Active || th.SelectedTask.FK_TaskStatus == (int)TaskHandler.TaskStatus.Reported)
			{
				this.Frame.Navigate(typeof(TaskEditPage));
			}
			else
			{
				ErrorHandler.CannotEditTask();
			}
		}

		//This code makes it possible to tap a Profile name and view it
		#region TappingProfileNames
		private void taskFetcherBind_Tapped(object sender, TappedRoutedEventArgs e)
		{
			var ph = ProfileHandler.GetInstance();
			var taskViewModel = pageRoot.DataContext as TaskDetailViewModel;

			if (taskViewModel != null)
			{
				ph.SelectedProfile = taskViewModel.Fetcher;
			}
			this.Frame.Navigate(typeof(ProfileDetailPage));
		}


		private void taskmasterBind_Tapped(object sender, TappedRoutedEventArgs e)
		{
			var ph = ProfileHandler.GetInstance();
			var taskViewModel = pageRoot.DataContext as TaskDetailViewModel;

			if (taskViewModel != null)
			{
				ph.SelectedProfile = taskViewModel.Taskmaster;
			}
			this.Frame.Navigate(typeof(ProfileDetailPage));
		}
		#endregion

		private void appBarButton_Click(object sender, RoutedEventArgs e)
		{
		    appBar.IsOpen = !appBar.IsOpen;
		}

	    #region AppBar Buttons
		private void profileButton_Click(object sender, RoutedEventArgs e)
		{
			ph.SelectedProfile = ph.CurrentLoggedInProfile;
			this.Frame.Navigate(typeof(ProfileDetailPage));
		}

		private async void messageHubButton_Click(object sender, RoutedEventArgs e)
		{
			await Task.Delay(50);
			this.Frame.Navigate(typeof(MessageHub));
		}

		private async void issuesButton_Click(object sender, RoutedEventArgs e)
		{
			await Task.Delay(50);
			this.Frame.Navigate(typeof(IssuesView));
		}

		private async void CreateTaskButton_Click(object sender, RoutedEventArgs e)
		{
			await Task.Delay(50);
			this.Frame.Navigate(typeof(TaskCreation));
		}

        private void profileLogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ph.LogOut();
        }

        private void NavigateToLogIn()
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void homeButton_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(LandingPage));
		}
		#endregion

		async private void AssignToTaskButton_Click(object sender, RoutedEventArgs e)
		{
			var viewModel = pageRoot.DataContext as TaskDetailViewModel;
			if (viewModel != null)
			{
				while (viewModel.Assigned == false)
				{
					await Task.Delay(500);
				}
				viewModel.Assigned = false;
				AssignToTaskButton.Visibility = Visibility.Collapsed;
				RefreshButtons();
			}
		}

		async private void ResignFromTaskButton_Click(object sender, RoutedEventArgs e)
		{
			var viewModel = pageRoot.DataContext as TaskDetailViewModel;
			if (viewModel != null)
			{
				while (viewModel.Resigned == false)
				{
					await Task.Delay(500);
				}
				viewModel.Resigned = false;
				ResignFromTaskButton.Visibility = Visibility.Collapsed;
				RefreshButtons();
			}
		}

		async private void MarkAsCompletedButton_Click(object sender, RoutedEventArgs e)
		{
			var viewModel = pageRoot.DataContext as TaskDetailViewModel;
			if (viewModel != null)
			{
				while (viewModel.MarkedCompleted == false)
				{
					await Task.Delay(500);
				}
				viewModel.MarkedCompleted = false;
				MarkAsCompletedButton.Visibility = Visibility.Collapsed;
				RefreshButtons();
			}
		}
	}
}
