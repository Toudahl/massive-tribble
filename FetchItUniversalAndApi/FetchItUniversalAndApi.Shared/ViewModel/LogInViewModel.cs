using System.Windows.Input;
using FetchItUniversalAndApi.Common;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.ViewModel
{
    class LogInViewModel
    {
        #region Propertied and Fields
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
        #endregion

        #region Methods
        public LogInViewModel()
        {
            ph = ProfileHandler.GetInstance();
        }

        

        private void LogIn()
        {
            ph.LogIn(new ProfileModel
            {
                ProfileName = Username,
                ProfilePassword = Password,
            });
        }
        #endregion
    }
}
