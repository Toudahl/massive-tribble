namespace FetchItUniversalAndApi.Models
{
    public struct PaymentModel
    {
        public int PaymentId { get; set; }
        public string FK_PaymentNotificationReferenceId { get; set; }
        public int FK_PaymentStatus { get; set; }
        public decimal PaymentAmount { get; set; }
        public int FK_PaymentForTask { get; set; }
        public int FK_PaymentType { get; set; }
    }
}
