using System.Collections.Generic;

namespace FetchItUniversalAndApi.Models
{
    public partial class PaymentStatusModel
    {
        public PaymentStatusModel()
        {
            this.Payments = new HashSet<PaymentModel>();
        }

        public int PaymentStatusId { get; set; }
        public string PaymentStatus { get; set; }

        public virtual ICollection<PaymentModel> Payments { get; set; }
    }
}
