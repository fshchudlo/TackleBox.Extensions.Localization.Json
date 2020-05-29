using System;
using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Options;

// ReSharper disable ConvertToLambdaExpression

namespace TackleBox.Extensions.Localization.Json.JsonLoader
{
    internal class JsonCache
    {
        private readonly JsonLocalizationOptions _options;
        private readonly ConcurrentDictionary<string, CacheEntry> _cache = new ConcurrentDictionary<string, CacheEntry>();
        private static readonly JsonDocument EmptyDocument = JsonDocument.Parse("{}");
        public JsonCache(IOptions<JsonLocalizationOptions> options)
        {
            _options = options.Value;
        }

        public JsonDocument GetOrAdd(string cacheKey, Func<object, JsonDocument> valueFactory)
        {
            return _cache.AddOrUpdate(cacheKey, key =>
            {
                return new CacheEntry(LoadWithDegradationConsideration(valueFactory, key, null), _options.CacheExpirationLimit);
            }, (key, entry) =>
            {
                return entry.IsExpired ? new CacheEntry(LoadWithDegradationConsideration(valueFactory, key, entry.Document), _options.CacheExpirationLimit) : entry;
            }).Document;
        }

        /// <summary>
        /// If <see cref="JsonLocalizationOptions.UseSilentDegradation"/> is set to true, then returns previously loaded resource on any exception
        /// </summary>
        private JsonDocument LoadWithDegradationConsideration(Func<object, JsonDocument> valueFactory, string key, JsonDocument? previousValue)
        {
            try
            {
                return valueFactory(key);
            }
            catch
            {
                if (_options.UseSilentDegradation)
                {
                    return previousValue ?? EmptyDocument;
                }
                throw;
            }
        }

        private class CacheEntry
        {
            private readonly DateTime _expiresAt;
            public JsonDocument Document { get; }
            public bool IsExpired => DateTime.UtcNow >= _expiresAt;
            public CacheEntry(JsonDocument document, TimeSpan expiration)
            {
                Document = document;
                _expiresAt = DateTime.UtcNow.AddSeconds(expiration.TotalSeconds);
            }
        }
    }
}
