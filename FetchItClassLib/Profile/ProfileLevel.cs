using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FetchItClassLib.Profile
{
    public partial class ProfileLevel
    {
        public ProfileLevel()
        {
            Profiles = new HashSet<ProfileModel>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProfileLevelId { get; set; }

        [Column("ProfileLevel")]
        [Required]
        [StringLength(20)]
        public string ProfileLevel1 { get; set; }

        public virtual ICollection<ProfileModel> Profiles { get; set; }
    }
}
