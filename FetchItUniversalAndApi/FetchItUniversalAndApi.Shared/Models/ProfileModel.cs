using System;

namespace FetchItUniversalAndApi.Models
{
    public class ProfileModel
    {
        private int _profileId;
        private string _profileName;
        private string _profileEmail;

        public int ProfileId
        {
            get { return _profileId; }
            set
            {
                if (_profileId == 0)
                {
                    _profileId = value;
                }
            }
        }

        public string ProfileName
        {
            get { return _profileName; }
            set
            {
                if (_profileName == null)
                {
                    _profileName = value;
                }
            }
        }

        public string ProfileAddress { get; set; }
        public string ProfilePhone { get; set; }
        public string ProfileMobile { get; set; }
        public string ProfilePassword { get; set; }

        public string ProfileEmail
        {
            get { return _profileEmail; }
            set
            {
                if (_profileEmail == null)
                {
                    _profileEmail = value;
                }
            }
        }

        public int FK_ProfileStatus { get; set; }
        public DateTime? ProfileLastLoggedIn { get; set; }
        public int FK_ProfileLevel { get; set; }
        public bool ProfileIsVerified { get; set; }
        public byte[] ProfileImage { get; set; }
        public string ProfileText { get; set; }
        public long ProfilePasswordSalt { get; set; }
        public int FK_ProfileVerificationType { get; set; }
        public long? ProfileAuthCode { get; set; }
        public string ProfileNewEmail { get; set; }
        public string ProfileNewPassword { get; set; }
        public byte ProfileCanReport { get; set; }
    }
}
