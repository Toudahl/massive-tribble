using System.Collections.Generic;

namespace FetchItUniversalAndApi.Models
{
    public partial class PaymentTypeModel
    {
        public PaymentTypeModel()
        {
            this.Payments = new HashSet<PaymentModel>();
        }

        public int PaymentTypeId { get; set; }
        public string PaymentType { get; set; }

        public virtual ICollection<PaymentModel> Payments { get; set; }
    }
}
