using System.ComponentModel.DataAnnotations;

namespace TransactionManagementSystem.Domain.Entities
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Banks { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    }
}
