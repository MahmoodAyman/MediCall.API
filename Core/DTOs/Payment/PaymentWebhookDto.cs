using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.DTOs.Payment
{
    public class PaymentWebhookDto
    {
        [JsonPropertyName("obj")]
        public PaymentWebhookObject? Obj { get; set; }

        [JsonPropertyName("hmac")]
        public string? Hmac { get; set; }
    }

    public class PaymentWebhookObject
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("pending")]
        public bool Pending { get; set; }

        [JsonPropertyName("amount_cents")]
        public int AmountCents { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("is_refunded")]
        public bool IsRefunded { get; set; }

        [JsonPropertyName("is_voided")]
        public bool IsVoided { get; set; }

        [JsonPropertyName("order")]
        public OrderInfo? Order { get; set; }
    }

    public class OrderInfo
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("extras")]
        public Extras? Extras { get; set; }
    }

    public class Extras
    {
        [JsonPropertyName("visit_id")]
        public int VisitId { get; set; }
    }
} 