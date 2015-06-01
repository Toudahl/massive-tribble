using System.Threading.Tasks;
using Windows.UI.Popups;
using FetchItUniversalAndApi.Common;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.View
{
	/// <summary>
	/// A basic page that provides characteristics common to most applications.
	/// </summary>
	public sealed partial class TaskEditPage : Page
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


		public TaskEditPage()
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
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedFrom(e);
		}

		#endregion

		private void goBackButton_Click(object sender, RoutedEventArgs e)
		{
			navigationHelper.GoBack();
		}

		/// <summary>
		/// A method that shows the user a MessageDialog, making him confirm that he wants to remove his own task from the marketplace, then calls the removeTask method.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		async private void RemoveTaskButton_Click(object sender, RoutedEventArgs e)
		{

			MessageDialog message = new MessageDialog("Are you sure you want to remove the task? This action will remove the Task from the marketplace, and take you back to the MainPage.", "Remove Task");
			message.Commands.Add(new UICommand(
				"Yes",
				command => RemoveTask()));

			message.Commands.Add(new UICommand(
				"No"));

			message.DefaultCommandIndex = 0;
			message.CancelCommandIndex = 1;

			await message.ShowAsync();
		}

		/// <summary>
		/// This method removes the selected task from the marketplace, and 
		/// </summary>
		async private void RemoveTask()
		{
			var th = TaskHandler.GetInstance();
			var taskToRemove = th.SelectedTask;
			taskToRemove.FK_TaskStatus = (int)TaskHandler.TaskStatus.Removed;
			try
			{
				th.Update(taskToRemove);
				await Task.Delay(500);
				this.Frame.Navigate(typeof(LandingPage));
			}
			catch (Exception)
			{
				ErrorHandler.SuspendingError(new TaskModel());
			}
		}

        #region AppBar Buttons
        private void profileButton_Click(object sender, RoutedEventArgs e)
        {
            var ph = ProfileHandler.GetInstance();
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
            var ph = ProfileHandler.GetInstance();
            ph.LogOut();
            this.Frame.Navigate(typeof(MainPage));
        }
        #endregion
	}
}

