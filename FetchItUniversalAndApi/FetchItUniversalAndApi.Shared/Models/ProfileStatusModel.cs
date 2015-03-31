using System;
using System.Collections.Generic;
using System.Text;

namespace FetchItUniversalAndApi.Models
{
    public partial class ProfileStatusModel
    {
        public ProfileStatusModel()
        {
            this.Profiles = new HashSet<ProfileModel>();
        }

        public int ProfileStatusId { get; set; }
        public string ProfileStatus { get; set; }

        public virtual ICollection<ProfileModel> Profiles { get; set; }
    }
}
