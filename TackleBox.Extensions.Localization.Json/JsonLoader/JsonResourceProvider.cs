using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TackleBox.Extensions.Localization.Json.JsonLoader
{
    /// <summary>
    /// </summary>
    internal class JsonResourceProvider
    {
        private readonly ILogger<JsonResourceProvider> _logger;
        private readonly JsonLoader _loader;
        private readonly JsonLocalizationOptions _options;
        private readonly JsonCache _resourceCache;

        /// <summary>
        /// </summary>
        public JsonResourceProvider(IOptions<JsonLocalizationOptions> options, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<JsonResourceProvider>();
            _options = options.Value;
            _resourceCache = new JsonCache(options);
            _loader = JsonLoader.Create(options, _logger);
        }

        /// <exception cref="UnableToLoadResourceFileException"></exception>
        public IList<string> GetAllResourceKeys(string resourceFileName, CultureInfo? culture)
        {
            var document = FetchResource(resourceFileName, culture);
            return document.RootElement.EnumerateObject().Select(e => e.Name).ToList();
        }

        /// <exception cref="KeyNotFoundException"></exception>
        public string? GetTranslation(string resourceFileName, string resourceKey, CultureInfo? culture)
        {
            var document = FetchResource(resourceFileName, culture);
            if (document.RootElement.TryGetProperty(resourceKey, out var entry))
            {
                return entry.GetString();
            }

            var message = $"Translation of '{resourceKey}' key for '{culture?.Name}' culture wasn't found at {resourceFileName} file.";
            if (_options.ThrowIfKeyDoesNotExist)
            {
                throw new KeyNotFoundException(message);
            }

            _logger.LogWarning(message);
            return null;
        }

        private JsonDocument FetchResource(string resourceFileName, CultureInfo? culture)
        {
            var cacheKey = $"{resourceFileName}|{culture?.Name}";
            return _resourceCache.GetOrAdd(cacheKey, key => _loader.LoadResource(resourceFileName, culture));
        }
    }
}