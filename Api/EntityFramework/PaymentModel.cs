//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Api.EntityFramework
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
