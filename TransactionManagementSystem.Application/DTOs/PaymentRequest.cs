namespace TransactionManagementSystem.Application.DTOs
{
    public class PaymentRequest
    {
        public string Email { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }
        public string Currency { get; set; }
        public string CallbackUrl { get; set; }
    }

    public class PaymentInitializationResponse
    {
        public bool Success { get; set; }
        public string Reference { get; set; }
        public string AuthorizationUrl { get; set; }
        public string AccessCode { get; set; }
        public string Message { get; set; }
    }

    public class PaymentVerificationResponse
    {
        public bool Success { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime? PaidAt { get; set; }
        public string Message { get; set; }
    }

    public class WithdrawalRequest
    {
        public decimal Amount { get; set; }
        public string RecipientName { get; set; }
        public string AccountNumber { get; set; }
        public string BankCode { get; set; }
        public string Currency { get; set; }
        public string Reason { get; set; }
    }

    public class WithdrawalResponse
    {
        public bool Success { get; set; }
        public string TransferCode { get; set; }
        public string Reference { get; set; }
        public string Message { get; set; }
    }

    // Paystack API Response Models
    public class PaystackResponse<T>
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class PaymentData
    {
        public string AuthorizationUrl { get; set; }
        public string AccessCode { get; set; }
        public string Reference { get; set; }
    }

    public class TransactionData
    {
        public string Reference { get; set; }
        public int Amount { get; set; }
        public string Status { get; set; }
        public DateTime? PaidAt { get; set; }
    }

    public class RecipientData
    {
        public string RecipientCode { get; set; }
        public string Name { get; set; }
    }

    public class TransferData
    {
        public string TransferCode { get; set; }
        public string Reference { get; set; }
        public string Status { get; set; }
    }
}

