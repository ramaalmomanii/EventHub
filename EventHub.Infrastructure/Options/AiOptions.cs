namespace EventHub.Infrastructure.Options
{
    public class AiOptions
    {
        public const string SectionName = "Ai";

        public string DefaultProvider { get; set; } = "openai";
        public OpenAiOptions OpenAi { get; set; } = new();
        public GeminiOptions Gemini { get; set; } = new();
    }

    public class OpenAiOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gpt-4o-mini";
    }

    public class GeminiOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gemini-2.0-flash";
    }
}
