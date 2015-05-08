using System;
using System.Collections.Generic;
using System.Text;
using FetchItUniversalAndApi.Handlers;
using FetchItUniversalAndApi.Models;


namespace FetchItUniversalAndApi.ViewModel
{
    class ProfileDetailViewModel
    {
        public ProfileHandler ProfileHandler { get; set; }
        public string selectedprofileName { get; set; }
        public string selectedprofileMobile { get; set; }
        public string selectedprofileAddress { get; set; }
        public string selectedprofileEmail { get; set; }
        public int selectedprofileId { get; set; }
        public int selectedprofileRating { get; set; }

       

        public ProfileDetailViewModel()
        {
            ProfileHandler = ProfileHandler.GetInstance();
            
            if (ProfileHandler.SelectedProfile!=null)
            {
            selectedprofileName = ProfileHandler.SelectedProfile.ProfileName;
            selectedprofileMobile = ProfileHandler.SelectedProfile.ProfileMobile;
            selectedprofileEmail = ProfileHandler.SelectedProfile.ProfileEmail;
            selectedprofileAddress = ProfileHandler.SelectedProfile.ProfileAddress;
            selectedprofileId = ProfileHandler.SelectedProfile.ProfileId;  
            
            }
            
            



        }

    }
}
