using System;
using System.Collections.Generic;
using System.Text;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.ViewModel
{
    class MainPageViewModel
    {
        private ProfileHandler ph;

        public ProfileModel LoggedInUser
        {
            get { return ph.CurrentLoggedInProfile; } 
        }

        public MainPageViewModel()
        {
            ph = ProfileHandler.GetInstance();
        }

    }
}
