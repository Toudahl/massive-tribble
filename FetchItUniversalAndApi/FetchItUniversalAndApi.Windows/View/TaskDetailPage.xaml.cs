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

namespace FetchItUniversalAndApi.View
{
	/// <summary>
	/// A basic page that provides characteristics common to most applications.
	/// </summary>
	public sealed partial class TaskDetailPage : Page
	{
		private NavigationHelper navigationHelper;
		private ObservableDictionary defaultViewModel = new ObservableDictionary();

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
		/// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
		/// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
		/// The navigation parameter is available in the LoadState method 
		/// in addition to page state preserved during an earlier session.

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedTo(e);

			var th = TaskHandler.GetInstance();
			var ph = ProfileHandler.GetInstance();

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

				//This LeaveCommentButton is only available ATM to fetchers or taskmasters of
				//the selected task
				LeaveCommentButton.Visibility = Visibility.Visible;

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
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedFrom(e);
		}

		#endregion

		private void goBackButton_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(LandingPage));
		}

		private void CreateFeedbackButton_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(CreateFeedbackPage));
		}

		async private void EditTaskButton_Click(object sender, RoutedEventArgs e)
		{
			var th = TaskHandler.GetInstance();
			if (th.SelectedTask.FK_TaskStatus == 1 || th.SelectedTask.FK_TaskStatus == 2)
			{
				this.Frame.Navigate(typeof(TaskEditPage));
			}
			else
			{
				MessageDialog message = new MessageDialog("You cannot edit the task in its current condition.", "Edit Task");
				await message.ShowAsync();
			}
		}

		private void CancelCommentButton_Click(object sender, RoutedEventArgs e)
		{
			CommentBorder.Visibility = Visibility.Collapsed;
		}

		async private void LeaveCommentButton_Click(object sender, RoutedEventArgs e)
		{
			CommentBorder.Visibility = Visibility.Visible;

			//Success message is a TextBlock control in the UI, which has a bind to the
			//TaskDetailViewModel, it gets set in VM when the comment has been successfully 
			//added to the database. Here it is used to make the CommentGrid dissappear
			//after a comment is added.
			while (SuccessMessage.Visibility == Visibility.Collapsed)
			{
				await Task.Delay(500);
			}
			CommentBorder.Visibility = Visibility.Collapsed;
		}
	}
}
