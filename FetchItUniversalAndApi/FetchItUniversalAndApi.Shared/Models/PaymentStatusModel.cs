using System;
using System.Collections.Generic;
using System.Text;

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
