using EventHub.Core.DTOs.Events;
using EventHub.Core.Exceptions;
using EventHub.Core.Interfaces.Services;
using EventHub.Infrastructure.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EventHub.Infrastructure.Services
{
    public class EventSummaryService : IEventSummaryService
    {
        private readonly IEventService _eventService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly AiOptions _options;

        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);

        public EventSummaryService(
            IEventService eventService,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            IOptions<AiOptions> options)
        {
            _eventService = eventService;
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _options = options.Value;
        }

        public async Task<EventSummaryDto> GetSummaryAsync(int eventId, string provider)
        {
            if (eventId <= 0)
                throw new ValidationException("Invalid event ID");

            var normalizedProvider = NormalizeProvider(provider);
            var cacheKey = $"summary_{eventId}_{normalizedProvider}";

            if (_cache.TryGetValue(cacheKey, out EventSummaryDto? cached) && cached != null)
                return cached;

            var ev = await _eventService.GetByIdAsync(eventId);
            if (ev == null)
                throw new NotFoundException($"Event with ID {eventId} not found");

            var prompt = BuildPrompt(ev);
            var summary = normalizedProvider == "gemini"
                ? await CallGeminiAsync(prompt)
                : await CallOpenAiAsync(prompt);

            var result = new EventSummaryDto
            {
                EventId = eventId,
                Summary = summary,
                Provider = normalizedProvider,
                GeneratedAt = DateTime.UtcNow
            };

            _cache.Set(cacheKey, result, CacheDuration);
            return result;
        }

        private static string NormalizeProvider(string provider)
        {
            var p = (provider ?? string.Empty).Trim().ToLowerInvariant();
            return p == "gemini" ? "gemini" : "openai";
        }

        private static string BuildPrompt(EventReadDto ev)
        {
            return $"""
                Write a concise, engaging summary (3-4 sentences) for this event.
                Focus on what attendees can expect. Do not use bullet points.

                Title: {ev.Title}
                Category: {ev.CategoryName}
                Location: {ev.Location}
                Start: {ev.StartDate:yyyy-MM-dd HH:mm}
                End: {ev.EndDate:yyyy-MM-dd HH:mm}
                Price: {ev.Price}
                Status: {ev.Status}
                Description: {ev.Description ?? "No description provided."}
                """;
        }

        private async Task<string> CallOpenAiAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(_options.OpenAi.ApiKey))
                throw new ValidationException("OpenAI API key is not configured. Set Ai:OpenAi:ApiKey in appsettings.");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _options.OpenAi.ApiKey);

            var body = new
            {
                model = _options.OpenAi.Model,
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful event assistant. Provide brief, friendly event summaries." },
                    new { role = "user", content = prompt }
                },
                max_tokens = 250,
                temperature = 0.7
            };

            var json = JsonSerializer.Serialize(body);
            var response = await client.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                new StringContent(json, Encoding.UTF8, "application/json"));

            var responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new ValidationException($"OpenAI request failed: {responseBody}");

            using var doc = JsonDocument.Parse(responseBody);
            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "Summary unavailable.";
        }

        private async Task<string> CallGeminiAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(_options.Gemini.ApiKey))
                throw new ValidationException("Gemini API key is not configured. Set Ai:Gemini:ApiKey in appsettings.");

            var client = _httpClientFactory.CreateClient();
            var model = _options.Gemini.Model;
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={_options.Gemini.ApiKey}";

            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    maxOutputTokens = 250,
                    temperature = 0.7
                }
            };

            var json = JsonSerializer.Serialize(body);
            var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new ValidationException($"Gemini request failed: {responseBody}");

            using var doc = JsonDocument.Parse(responseBody);
            return doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "Summary unavailable.";
        }
    }
}
