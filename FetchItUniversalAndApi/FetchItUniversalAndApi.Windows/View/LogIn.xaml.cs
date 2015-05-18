// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

using System;
using System.Diagnostics;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using FetchItUniversalAndApi.Common;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.ViewModel;

namespace FetchItUniversalAndApi.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ProfileHandler ph;
        public MainPage()
        {
            this.InitializeComponent();
            ph = ProfileHandler.GetInstance();
        }

        private void RegisterButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RegisterUser));
        }

        private void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            DoLogIn();
        }

        private async void DoLogIn()
        {
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(500);
                if (ph.CurrentLoggedInProfile != null)
                {
                    Frame.Navigate(typeof(LandingPage));
                    break;
                }
            }
            //if (ph.CurrentLoggedInProfile == null)
            //{
            //    var msg = new MessageDialog("Failed to log in within the allowed time.\nCheck your internet connectivity, and try again").ShowAsync();
            //}
        }
    }
}
