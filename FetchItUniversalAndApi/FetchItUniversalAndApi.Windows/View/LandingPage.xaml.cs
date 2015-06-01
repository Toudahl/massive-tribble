using System.Threading.Tasks;
using FetchItUniversalAndApi.Common;
using System;
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
		}

		private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
		{
		}

		
		private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
		{
		}

        
		private void profileDetailsButton_Click(object sender, RoutedEventArgs e)
		{
			var ph = ProfileHandler.GetInstance();
			ph.SelectedProfile = ph.CurrentLoggedInProfile;
			this.Frame.Navigate(typeof(ProfileDetailPage));
		}

		private void CreateTaskButton_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(TaskCreation));
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

		private void profileLogoutButton_Click(object sender, RoutedEventArgs e)
		{
			var ph = ProfileHandler.GetInstance();
			ph.LogOut();
			this.Frame.Navigate(typeof(MainPage));
		}

        private void appBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (appBar.IsOpen == true)
            {
                appBar.IsOpen = false;
            }
            else
            {
                appBar.IsOpen = true;
            }
        }

        
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

       


    }
}
