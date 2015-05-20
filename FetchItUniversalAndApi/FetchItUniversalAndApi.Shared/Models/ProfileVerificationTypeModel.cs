using System.Collections.Generic;

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
