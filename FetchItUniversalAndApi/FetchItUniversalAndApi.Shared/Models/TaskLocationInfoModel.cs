namespace FetchItUniversalAndApi.Models
{
    public partial class TaskLocationInfoModel
    {
        public int TaskLocationInfoId { get; set; }
        public int FK_TaskId { get; set; }
        public string TaskLocationAddress { get; set; }

        public virtual TaskModel Task { get; set; }
    }
}
