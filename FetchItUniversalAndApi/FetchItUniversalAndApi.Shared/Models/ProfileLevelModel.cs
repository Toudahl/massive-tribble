using System.Collections.Generic;

namespace FetchItUniversalAndApi.Models
{
    public partial class ProfileLevelModel
    {
        public ProfileLevelModel()
        {
            this.Profiles = new HashSet<ProfileModel>();
        }

        public int ProfileLevelId { get; set; }
        public string ProfileLevelTitle { get; set; }

        public virtual ICollection<ProfileModel> Profiles { get; set; }
    }
}
