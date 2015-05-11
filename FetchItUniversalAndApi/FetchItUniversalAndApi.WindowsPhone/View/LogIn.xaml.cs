// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using FetchItUniversalAndApi.Handlers;

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

        private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            await Task.Delay(1500);
            if (ph.CurrentLoggedInProfile != null)
            {
                Frame.Navigate(typeof(LandingPage));
            }
        }
    }
}
