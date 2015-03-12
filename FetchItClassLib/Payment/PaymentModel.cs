using System.ComponentModel.DataAnnotations.Schema;
using FetchItClassLib.Profile;
using FetchItClassLib.Task;

namespace FetchItClassLib.Payment
{
    public partial class PaymentModel
    {
        public int PaymentId { get; set; }

        public int PaymentStatus { get; set; }

        public int PaymentFromProfile { get; set; }

        public int PaymentToProfile { get; set; }

        [Column(TypeName = "money")]
        public decimal PaymentAmount { get; set; }

        public int PaymentForTask { get; set; }

        public int PaymentType { get; set; }

        public virtual TaskModel Task { get; set; }

        public virtual ProfileModel Profile { get; set; }

        public virtual PaymentStatus PaymentStatus1 { get; set; }

        public virtual ProfileModel Profile1 { get; set; }

        public virtual PaymentType PaymentType1 { get; set; }
    }
}
