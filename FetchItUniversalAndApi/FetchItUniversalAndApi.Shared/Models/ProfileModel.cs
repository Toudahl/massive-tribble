using System;
using System.Collections.Generic;

namespace FetchItUniversalAndApi.Models
{
    public class ProfileModel
    {

        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileAddress { get; set; }
        public string ProfilePhone { get; set; }
        public string ProfileMobile { get; set; }
        public string ProfilePassword { get; set; }
        public string ProfileEmail { get; set; }
        public int FK_ProfileStatus { get; set; }
        public Nullable<DateTime> ProfileLastLoggedIn { get; set; }
        public int FK_ProfileLevel { get; set; }
        public bool ProfileIsVerified { get; set; }
        public byte[] ProfileImage { get; set; }
        public string ProfileText { get; set; }
        public long ProfilePasswordSalt { get; set; }
        public int FK_ProfileVerificationType { get; set; }
        public Nullable<long> ProfileAuthCode { get; set; }
        public string ProfileNewEmail { get; set; }
        public string ProfileNewPassword { get; set; }
        public byte ProfileCanReport { get; set; }

        //public virtual ICollection<CommentModel> Comments { get; set; }
        //public virtual ICollection<IssueModel> IssueCreator { get; set; }
        //public virtual ICollection<IssueModel> IssueTarget { get; set; }
        //public virtual ICollection<NotificationModel> NotificationFromProfile { get; set; }
        //public virtual ICollection<NotificationModel> NotificationToProfile { get; set; }
        //public virtual ProfileLevelModel ProfileLevel { get; set; }
        //public virtual ProfileStatusModel ProfileStatus { get; set; }
        //public virtual ProfileVerificationTypeModel ProfileVerificationType { get; set; }
        //public virtual ICollection<ReportModel> ReportedProfile { get; set; }
        //public virtual ICollection<ReportModel> ReportingProfile { get; set; }
        //public virtual ICollection<SessionModel> Sessions { get; set; }
        //public virtual ICollection<TaskModel> TaskFetcherProfile { get; set; }
        //public virtual ICollection<TaskModel> TaskMasterProfile { get; set; }
    }
}
