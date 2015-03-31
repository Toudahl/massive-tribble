using System;
using System.Collections.Generic;
using System.Text;

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
