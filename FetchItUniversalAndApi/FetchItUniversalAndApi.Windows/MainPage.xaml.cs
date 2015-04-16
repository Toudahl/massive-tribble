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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi
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

            ProfileModel test = new ProfileModel();
            test.ProfileName = "Morten";
            test.ProfileAddress = "fake";
            test.ProfileMobile = "28336567";
            test.ProfilePassword = "password";
            test.ProfileEmail = "toudahl@gmail.com";
            test.ProfileCanReport = 1;

            ph = ProfileHandler.GetInstance();

            //ph.LogIn(test);

        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ph.Delete(ph.CurrentLoggedInProfile);
        }
    }
}
