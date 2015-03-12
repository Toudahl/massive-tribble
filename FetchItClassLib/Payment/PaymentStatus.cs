using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FetchItClassLib.Payment
{
    [Table("PaymentStatuses")]
    public partial class PaymentStatus
    {
        public PaymentStatus()
        {
            Payments = new HashSet<PaymentModel>();
        }

        public int PaymentStatusId { get; set; }

        [Column("PaymentStatus")]
        [Required]
        [StringLength(20)]
        public string PaymentStatus1 { get; set; }

        public virtual ICollection<PaymentModel> Payments { get; set; }
    }
}
