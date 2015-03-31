using System;
using System.Collections.Generic;
using System.Text;
using FetchItUniversalAndApi.Handlers.Interfaces;
using FetchItUniversalAndApi.Models;

namespace FetchItUniversalAndApi.Handlers
{
    class ProfileHandler: IDelete, ICreate, ISuspend, IDisable, IUpdate, ISearch
    {
        private ProfileModel _currentLoggedInProfile;
        private ProfileModel _selectedProfile;
        private static ProfileHandler _handler;
        private static Object _lockObject = new object();

        public ProfileModel CurrentLoggedInProfile
        {
            get { return _currentLoggedInProfile; }
            private set { _currentLoggedInProfile = value; }
        }

        public ProfileModel SelectedProfile
        {
            get { return _selectedProfile; }
            set { _selectedProfile = value; }
        }

        private ProfileHandler()
        {
            
        }

        public static ProfileHandler GetInstance()
        {
            lock (_lockObject)
            {
                if (_handler == null)
                {
                    _handler = new ProfileHandler();
                }
                return _handler;
            }
        }

        public void Delete(object obj)
        {
            throw new NotImplementedException();
        }

        public void Create(object obj)
        {
            throw new NotImplementedException();
        }

        public void Suspend(object obj)
        {
            throw new NotImplementedException();
        }

        public void Disable(object obj)
        {
            throw new NotImplementedException();
        }

        public void Update(object obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> Search(object obj)
        {
            throw new NotImplementedException();
        }

        public async void LogIn(ProfileModel profile)
        {
            if (profile != null)
            {
                CurrentLoggedInProfile = profile;
            }
        }

        public void LogOut()
        {
            throw new NotImplementedException();
        }

    }
}
