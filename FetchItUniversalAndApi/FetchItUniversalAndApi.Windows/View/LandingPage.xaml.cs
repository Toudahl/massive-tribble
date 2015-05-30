using System.Threading.Tasks;
using FetchItUniversalAndApi.Common;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using FetchItUniversalAndApi.Handlers;

namespace FetchItUniversalAndApi.View
{
	/// <summary>
	/// A basic page that provides characteristics common to most applications.
	/// </summary>
	public sealed partial class LandingPage : Page
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


		public LandingPage()
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

		#endregion

		private void profileDetailsButton_Click(object sender, RoutedEventArgs e)
		{
			var ph = ProfileHandler.GetInstance();
			ph.SelectedProfile = ph.CurrentLoggedInProfile;
			this.Frame.Navigate(typeof(ProfileDetailPage));
		}

		private void CreateTaskButton_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(TaskDetailPage));
		}

		private void refreshMarketplaceButton_Click(object sender, RoutedEventArgs e)
		{
			coolDown();
		}

		private async void coolDown()
		{
			refreshMarketplaceButton.IsEnabled = false;
			await Task.Delay(5000);
			refreshMarketplaceButton.IsEnabled = true;
		}

		private void marketplaceListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.Frame.Navigate(typeof(TaskDetailPage));
		}

		private void MessageHubButton_OnClickButton_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(MessageHub));
		}

		private void issuePageButton_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(IssuesView));
		}

		private void notificationsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.Frame.Navigate(typeof(MessageHub));
		}

		private void userActiveTasksListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.Frame.Navigate(typeof(TaskDetailPage));
		}

        private void appbarButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof (BasicPage1));
        }
	}
}
