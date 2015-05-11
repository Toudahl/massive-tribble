using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Windows.UI.Popups;
using FetchItUniversalAndApi.Common;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.ViewModel
{
    class LogInViewModel
    {
        private ProfileHandler ph;
        private ICommand _logInCommand;
        private string _username;
        private string _password;

        public ProfileModel LoggedInUser
        {
            get
            {
                return ph.CurrentLoggedInProfile;
            }
        }

        public LogInViewModel()
        {
            ph = ProfileHandler.GetInstance();
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

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
            ph.LogIn(new ProfileModel
            {
                ProfileName = Username,
                ProfilePassword = Password,
            });
            
        }

    }
}
