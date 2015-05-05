using System;
using System.Collections.Generic;
using System.Text;
using FetchItUniversalAndApi.Handlers;


namespace FetchItUniversalAndApi.ViewModel
{
    class ProfileDetailViewModel
    {
        public ProfileHandler Ph { get; set; }
        public string selectedprofileName { get; set; }
        public string selectedprofileMobile { get; set; }
        public string selectedprofileAddress { get; set; }
        public string selectedprofileEmail { get; set; }
        public int selectedprofileId { get; set; }


        public ProfileDetailViewModel()
        {
            selectedprofileName = Ph.SelectedProfile.ProfileName;
            selectedprofileMobile = Ph.SelectedProfile.ProfileMobile;
            selectedprofileEmail = Ph.SelectedProfile.ProfileEmail;
            selectedprofileAddress = Ph.SelectedProfile.ProfileAddress;
            selectedprofileId = Ph.SelectedProfile.ProfileId;



        }

    }
}
