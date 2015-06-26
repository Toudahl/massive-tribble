using System.Threading.Tasks;
using FetchItUniversalAndApi.Common;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.ViewModel;

namespace FetchItUniversalAndApi.View
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class TaskCreation : Page
    {
        private ProfileHandler ph;
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


        public TaskCreation()
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

        private void buttonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (LandingPage));
        }

        private void buttonCreate_OnClick(object sender, RoutedEventArgs e)
        {
            GoToLandingPageAfterCreation();
        }

        private async void GoToLandingPageAfterCreation()
        {
            for (int i = 0; i < 10; i++)
            {
                if (TaskCreationViewModel.CreationSuccess)
                {
                    Frame.Navigate(typeof(LandingPage));
                    break;
                }
                await Task.Delay(500);
            }
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
        #endregion
    }
}
