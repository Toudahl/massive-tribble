namespace FetchItUniversalAndApi.Models
{
    public struct IssueDetailModel
    {
        public int IssueDetailId { get; set; }
        public int FK_Issue { get; set; }
        public bool IssueDetailFromCreator { get; set; }
        public byte[] IssueImage { get; set; }
        public string IssueDetailText { get; set; }
    }
}
