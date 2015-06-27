namespace FetchItUniversalAndApi.Models
{
    public struct TaskLocationInfoModel
    {
        public int TaskLocationInfoId { get; set; }
        public int FK_TaskId { get; set; }
        public string TaskLocationAddress { get; set; }
    }
}
