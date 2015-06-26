using System.Threading.Tasks;
using Windows.UI.Xaml;
using FetchItUniversalAndApi.Common;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.ViewModel;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace FetchItUniversalAndApi.View
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MessageHub : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private ProfileHandler ph;

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public MessageHub()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            feedbackListView.Visibility = Visibility.Collapsed;
            feedbackStackPanel.Visibility = Visibility.Collapsed;
            ph = ProfileHandler.GetInstance();
            ph.LogOutEvent += NavigateToLogIn;
        }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        
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

        private void feedbackButton_Click(object sender, RoutedEventArgs e) 
        {
            feedbackListView.Visibility =  Visibility.Visible;
            feedbackStackPanel.Visibility = Visibility.Visible;
            notificationsListView.Visibility = Visibility.Collapsed;
            NotificationStackPanel.Visibility = Visibility.Collapsed;
        }

        private void notificationsButton_Click(object sender, RoutedEventArgs e)
        {
            feedbackListView.Visibility = Visibility.Collapsed;
            feedbackStackPanel.Visibility = Visibility.Collapsed;
            notificationsListView.Visibility = Visibility.Visible;
            NotificationStackPanel.Visibility = Visibility.Visible;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof (LandingPage));
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
    }
}
