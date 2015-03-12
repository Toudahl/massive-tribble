using System;
using System.ComponentModel.DataAnnotations;
using FetchItClassLib.Profile;
using FetchItClassLib.Task;

namespace FetchItClassLib.Comment
{
    public partial class CommentModel
    {
        public int CommentId { get; set; }

        [Required]
        [StringLength(300)]
        public string CommentText { get; set; }

        public DateTime CommentDateCreated { get; set; }

        public int CommentCreator { get; set; }

        public int CommentTask { get; set; }

        public virtual ProfileModel Profile { get; set; }

        public virtual TaskModel Task { get; set; }
    }
}
