
using System.Linq;
using FetchItUniversalAndApi.Handlers;

namespace FetchItUniversalAndApi.Models
{
    public partial class CommentModel
    {
        public int CommentId { get; set; }
        public string FK_CommentNotificationReferenceId { get; set; }
        public string CommentText { get; set; }
        public System.DateTime CommentTimeCreated { get; set; }
        public int FK_CommentCreator { get; set; }
        public int FK_CommentTask { get; set; }

        public virtual ProfileModel Profile { get; set; }
        public virtual TaskModel Task { get; set; }

	    public override string ToString()
	    {
		    var ph = ProfileHandler.GetInstance();
		    var profiles = ph.AllProfiles;
		    var profileName = profiles.Where(profile => profile.ProfileId == FK_CommentCreator).Select(profile => profile.ProfileName).First();

		    if (profileName != null)
		    {
				return profileName + ": " + CommentText;
		    }
		    else
		    {
			    return CommentText;
		    }
	    }
    }
}
