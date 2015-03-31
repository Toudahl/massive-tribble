using System;
using System.Collections.Generic;
using System.Text;

namespace FetchItUniversalAndApi.Models
{
    public partial class PaymentModel
    {
        public int PaymentId { get; set; }
        public string FK_PaymentNotificationReferenceId { get; set; }
        public int FK_PaymentStatus { get; set; }
        public decimal PaymentAmount { get; set; }
        public int FK_PaymentForTask { get; set; }
        public int FK_PaymentType { get; set; }

        public virtual PaymentStatusModel PaymentStatus { get; set; }
        public virtual PaymentTypeModel PaymentType { get; set; }
        public virtual TaskModel Task { get; set; }
    }
}
