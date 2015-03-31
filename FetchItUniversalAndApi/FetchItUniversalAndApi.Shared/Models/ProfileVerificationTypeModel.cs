using System;
using System.Collections.Generic;
using System.Text;

namespace FetchItUniversalAndApi.Models
{
    public partial class ProfileVerificationTypeModel
    {
        public ProfileVerificationTypeModel()
        {
            this.Profiles = new HashSet<ProfileModel>();
        }

        public int ProfileVerificationTypeId { get; set; }
        public string ProfileVerificationType { get; set; }

        public virtual ICollection<ProfileModel> Profiles { get; set; }
    }
}
