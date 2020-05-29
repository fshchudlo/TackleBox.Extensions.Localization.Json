using System;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace TackleBox.Extensions.Localization.Json.JsonLoader
{
    internal static class LoggerExtensions
    {
        internal static void LogEmptyCultureWarning(this ILogger logger)
        {
            logger.LogWarning($"Culture can't be detected from parameters nor {nameof(JsonLocalizationOptions)}.{nameof(JsonLocalizationOptions.DefaultCulture)} option. Empty culture is used.");
        }

        internal static void LogLoadingError(this ILogger logger, string resourceFileName, string address, CultureInfo culture, Exception exception)
        {
            logger.LogError(exception, $"{nameof(JsonResourceProvider)} '{resourceFileName}' loading failed from '{address}' for culture '{culture.Name}'.");
        }

        internal static void LogLoading(this ILogger logger, string resourceFileName, string address, CultureInfo culture)
        {
            logger.LogDebug($"{nameof(JsonResourceProvider)} loaded '{resourceFileName}' from '{address}' for culture '{culture.Name}'.");
        }
    }
}
