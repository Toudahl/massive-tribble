using FetchItClassLib.Payment;

namespace FetchItClassLib
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PaymentType
    {
        public PaymentType()
        {
            Payments = new HashSet<PaymentModel>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PaymentTypeId { get; set; }

        [Column("PaymentType")]
        [Required]
        [StringLength(20)]
        public string PaymentType1 { get; set; }

        public virtual ICollection<PaymentModel> Payments { get; set; }
    }
}
