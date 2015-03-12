using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FetchItClassLib.Profile
{
    [Table("ProfileStatuses")]
    public partial class ProfileStatus
    {
        public ProfileStatus()
        {
            Profiles = new HashSet<ProfileModel>();
        }

        [Key]
        public int StatusId { get; set; }

        [Required]
        [StringLength(10)]
        public string Status { get; set; }

        public virtual ICollection<ProfileModel> Profiles { get; set; }
    }
}
