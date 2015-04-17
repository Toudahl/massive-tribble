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
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace FetchItUniversalAndApi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IssueHandler ih;
        public MainPage()
        {

            this.InitializeComponent();
           
        }
    }
}
