using System.Threading.Tasks;
using FetchItUniversalAndApi.Common;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using FetchItUniversalAndApi.Handlers;

namespace FetchItUniversalAndApi.View
{
	
	public sealed partial class LandingPage : Page
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


		public LandingPage()
		{
			this.InitializeComponent();
			this.navigationHelper = new NavigationHelper(this);
			this.navigationHelper.LoadState += navigationHelper_LoadState;
			this.navigationHelper.SaveState += navigationHelper_SaveState;
		    ph = ProfileHandler.GetInstance();
		    ph.LogOutEvent += NavigateToLogIn;
		}

		private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
		{
		}

		private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
		{
		}

        /// <summary>
        /// Makes the AppBarButton unabled for 3 seconds to avoid too many needless calls to server
        /// </summary>
        /// <param name="clickedButton">The AppBarButton being clicked</param>
		private async void coolDown(AppBarButton clickedButton)
		{
            clickedButton.IsEnabled = false;
			await Task.Delay(3000);
            clickedButton.IsEnabled = true;
		}

        #region Appbar Buttons OnClick
        /// <summary>
        /// Makes sure that the button cannot be clicked more than every 3 seconds
        /// </summary>
        /// <param name="sender">The UI object calling this event</param>
        /// <param name="e">Event arguments provided. Not used.</param>
        private void refreshButtons_Click(object sender, RoutedEventArgs e)
        {
            if (sender is AppBarButton)
            {
                coolDown((AppBarButton)sender);
            }
        }

        private void appBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (appBar.IsOpen)
            {
                appBar.IsOpen = false;
            }
            else
            {
                appBar.IsOpen = true;
            }
        }

        #region AppBar Buttons
        private void profileButton_Click(object sender, RoutedEventArgs e)
        {
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
        #endregion

        #region ListView OnSelectionChanged
        private async void marketplaceListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Task.Delay(50);
            this.Frame.Navigate(typeof(TaskDetailPage));
        }

        private async void notificationsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Task.Delay(50);
            this.Frame.Navigate(typeof(MessageHub));
        }

        private async void userActiveTasksListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Task.Delay(50);
            this.Frame.Navigate(typeof(TaskDetailPage));
        }
        #endregion

        #region Notifications Pointer actions
        /// <summary>
        /// These handle the Ponter Enter and Exit change of the background in the grids that contain the NotificationsList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Grid_PointerEntered01(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Column0Row1Grid.Background = new SolidColorBrush(Color.FromArgb(255, 120, 210, 255));
            Column0Row2Grid.Background = new SolidColorBrush(Color.FromArgb(255, 120, 210, 255));
        }

        private void Column0Row1Grid_PointerExited01(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Column0Row1Grid.Background = new SolidColorBrush();
            Column0Row2Grid.Background = new SolidColorBrush();
        }

        
        private void Column0Row2Grid_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Column0Row1Grid.Background = new SolidColorBrush(Color.FromArgb(255, 120, 210, 255));
            Column0Row2Grid.Background = new SolidColorBrush(Color.FromArgb(255, 120, 210, 255));
        }

        private void Column0Row2Grid_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Column0Row1Grid.Background = new SolidColorBrush();
            Column0Row2Grid.Background = new SolidColorBrush();
        }
        #endregion

        #region Tasks Pointer actions
        /// <summary>
        /// These handle the Ponter Enter and Exit change of the background in the grids that contain the MyTasks listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Column1Row1Grid_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Column1Row1Grid.Background = new SolidColorBrush(Color.FromArgb(255, 120, 210, 255));
            Column1Row2Grid.Background = new SolidColorBrush(Color.FromArgb(255, 120, 210, 255));
        }

        private void Column1Row1Grid_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Column1Row1Grid.Background = new SolidColorBrush();
            Column1Row2Grid.Background = new SolidColorBrush();
        }

        private void Column1Row2Grid_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Column1Row1Grid.Background = new SolidColorBrush(Color.FromArgb(255, 120, 210, 255));
            Column1Row2Grid.Background = new SolidColorBrush(Color.FromArgb(255, 120, 210, 255));
        }

        private void Column1Row2Grid_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Column1Row1Grid.Background = new SolidColorBrush();
            Column1Row2Grid.Background = new SolidColorBrush();
        }

        #endregion

        #region Marketplace actions
        private void Column2Row1Grid_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Column2Row1Grid.Background = new SolidColorBrush(Color.FromArgb(255, 120, 210, 255));
            Column2Row2Grid.Background = new SolidColorBrush(Color.FromArgb(255, 120, 210, 255));
        }

        private void Column2Row1Grid_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Column2Row1Grid.Background = new SolidColorBrush();
            Column2Row2Grid.Background = new SolidColorBrush();
        }

        private void Column2Row2Grid_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Column2Row1Grid.Background = new SolidColorBrush(Color.FromArgb(255, 120, 210, 255));
            Column2Row2Grid.Background = new SolidColorBrush(Color.FromArgb(255, 120, 210, 255));
        }

        private void Column2Row2Grid_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Column2Row1Grid.Background = new SolidColorBrush();
            Column2Row2Grid.Background = new SolidColorBrush();
        }
        #endregion
	}
}
