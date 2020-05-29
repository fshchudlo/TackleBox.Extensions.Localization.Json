using System;
using System.Globalization;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace TackleBox.Extensions.Localization.Json.JsonLoader
{
    /// <inheritdoc />
    internal class HttpJsonLoader : JsonLoader
    {
        private readonly JsonLocalizationOptions _options;
        private readonly ILogger _logger;

        /// <summary>
        /// </summary>
        public HttpJsonLoader(JsonLocalizationOptions options, ILogger logger)
        {
            _options = options;
            _logger = logger;
        }

        /// <inheritdoc />
        /// <exception cref="UnableToLoadResourceFileException"></exception>
        internal override JsonDocument LoadResource(string resourceFileName, CultureInfo? culture)
        {
            var cultureToUse = string.IsNullOrEmpty(culture?.Name) ? _options.DefaultCulture : culture;
            if (cultureToUse == null)
            {
                cultureToUse = new CultureInfo("");
                _logger.LogEmptyCultureWarning();
            }
            var address = _options.ResourcePathBuilder(_options, resourceFileName, cultureToUse);
            using var client = new WebClient();
            try
            {
                _logger.LogLoading(resourceFileName, address, cultureToUse);
                return JsonDocument.Parse(client.DownloadString(address));
            }
            catch (Exception e)
            {
                _logger.LogLoadingError(resourceFileName, address, cultureToUse, e);
                throw new UnableToLoadResourceFileException(resourceFileName, cultureToUse.Name, e);
            }
        }
    }
}