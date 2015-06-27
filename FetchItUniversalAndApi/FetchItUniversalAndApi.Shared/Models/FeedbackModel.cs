namespace FetchItUniversalAndApi.Models
{
    public struct FeedbackModel
    {
        public int FeedbackId { get; set; }
        public byte FeedbackRating { get; set; }
        public string FK_FeedbackNotificationReferenceId { get; set; }
        public string FeedbackComment { get; set; }
        public int FK_FeedbackForTask { get; set; }
        public int FK_FeedbackStatus { get; set; }

        public override string ToString()
        {
            return "Rating: " + FeedbackRating + "/nComment: " + FeedbackComment;
            
        }
    }
}
