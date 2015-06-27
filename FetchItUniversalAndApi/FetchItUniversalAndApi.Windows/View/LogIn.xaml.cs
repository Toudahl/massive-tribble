// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

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
            ph.LogInEvent += NavigateToLanding;
        }

        private void RegisterButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RegisterUser));
        }

        private void NavigateToLanding()
        {
            Frame.Navigate(typeof (LandingPage));
        }
    }
}
