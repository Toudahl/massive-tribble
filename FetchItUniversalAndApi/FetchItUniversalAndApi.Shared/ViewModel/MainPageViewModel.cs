using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using FetchItUniversalAndApi.Common;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.ViewModel
{
    class MainPageViewModel
    {
        private ProfileHandler ph;
        private ICommand _logInCommand;

        public ProfileModel LoggedInUser
        {
            get { return ph.CurrentLoggedInProfile; } 
        }

        public MainPageViewModel()
        {
            ph = ProfileHandler.GetInstance();
        }

        public string Username { get; set; }
        public string Password { get; set; }

        public ICommand LogInCommand
        {
            get
            {
                if (_logInCommand == null)
                {
                    _logInCommand = new RelayCommand(LogIn);
                }
                return _logInCommand;
            }
        }

        private void LogIn()
        {
            try
            {
                ph.LogIn(new ProfileModel
                {
                    ProfileName = Username,
                    ProfilePassword = Password,
                });

            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}
