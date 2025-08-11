using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TransactionManagementSystem.Application.DTOs.PaystackDtos
{
    public class InitiateResponse
    {

        [JsonPropertyName("status")]
        public bool Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("authorization_url")]
        public string AuthorizationUrl { get; set; }

        [JsonPropertyName("access_code")]
        public string AccessCode { get; set; }

        [JsonPropertyName("reference")]
        public string Reference { get; set; }
    }

    public class PaystackInitateRequest
    {
        [Required]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required]
        [JsonPropertyName("amount")]
        [Range(100, 10000000, ErrorMessage = "Amount must be between {1} and {2}")]
        public string Amount { get; set; }
    }

    public class PaystackVerifyResponse
    {
        [JsonPropertyName("status")]
        public bool Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

    }
}
