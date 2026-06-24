namespace EventHub.Core.DTOs.Events
{
    public record EventSummaryDto
    {
        public int EventId { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
    }
}
