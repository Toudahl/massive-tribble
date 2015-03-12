using System.ComponentModel.DataAnnotations;
using FetchItClassLib.Profile;

namespace FetchItClassLib.Feedback
{
    public partial class FeedbackModel
    {
        public int FeedbackId { get; set; }

        public int FeedbackRating { get; set; }

        [Required]
        [StringLength(300)]
        public string FeedbackComment { get; set; }

        public int FeedbackCreator { get; set; }

        public int FeedbackTarget { get; set; }

        public bool FeedbackIsSuspended { get; set; }

        public virtual ProfileModel Profile { get; set; }

        public virtual ProfileModel Profile1 { get; set; }
    }
}
