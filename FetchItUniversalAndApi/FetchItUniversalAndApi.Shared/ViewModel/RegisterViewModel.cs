using System.Windows.Input;
using FetchItUniversalAndApi.Common;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.ViewModel
{
    // Author: Morten Toudahl
    class RegisterViewModel
    {
        private ICommand _registerCommand;
        private ProfileHandler _ph;

        public RegisterViewModel()
        {
            _ph = ProfileHandler.GetInstance();
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public string Email { get; set; }
        public string EmailConfirm { get; set; }
        public string Address { get; set; }
        public int MobilePhone { get; set; }
        public static bool CreationSuccess { get; private set; }

        public ICommand RegisterCommand
        {
            get
            {
                if (_registerCommand == null)
                {
                    _registerCommand = new RelayCommand(Register);
                }
                return _registerCommand;
            }
        }


        private void Register()
        {
            CreationSuccess = false;
            var errorMessage = "";
            if (Password != PasswordConfirm)
            {
                errorMessage += "Passwords do not match";
            }
            if (Email != EmailConfirm)
            {
                errorMessage += "\nEmails do not match";
            }
            if (Address == null)
            {
                errorMessage += "\nAddress has not been filled in";
            }
            if (MobilePhone == 0)
            {
                errorMessage += "\nYou must enter a mobile phone number";
            }

            if (errorMessage != "")
            {
                ErrorHandler.DisplayErrorMessage(errorMessage,"Validation failed");
            }
            else
            {
                var newProfile = new ProfileModel
                {
                    ProfileName = Username,
                    ProfilePassword = Password,
                    ProfileEmail = Email,
                    ProfileAddress = Address,
                    ProfileMobile = MobilePhone.ToString()
                };

                _ph.Create(newProfile);
                CreationSuccess = true;
            }
        }
    }
}
