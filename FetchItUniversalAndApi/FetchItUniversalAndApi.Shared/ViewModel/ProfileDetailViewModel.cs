using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.ApplicationModel.Calls;
using FetchItUniversalAndApi.Annotations;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;


namespace FetchItUniversalAndApi.ViewModel
{
    class ProfileDetailViewModel:INotifyPropertyChanged
    {
        #region properties
        public ProfileHandler ProfileHandler { get; set; }
        public string selectedprofileName { get; set; }
        public string selectedprofileMobile { get; set; }
        public string selectedprofileAddress { get; set; }
        public string selectedprofileEmail { get; set; }
        public int selectedprofileId { get; set; }
        public int selectedprofileRating { get; set; }
        public ProfileModel SelectedProfile { get; set; }
        public int selectedProfileRating { get; set; }
        public string selectedProfileDescription { get; set; }
        #endregion



        public ProfileDetailViewModel()
        {
            ProfileHandler = ProfileHandler.GetInstance();

            
                SelectedProfile = ProfileHandler.CurrentLoggedInProfile;
                selectedprofileName = SelectedProfile.ProfileName;
                selectedprofileMobile = SelectedProfile.ProfileMobile;
                selectedprofileEmail = SelectedProfile.ProfileEmail;
                selectedprofileAddress = SelectedProfile.ProfileAddress;
                selectedprofileId = SelectedProfile.ProfileId;

            





        }
        #region prop changed
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}