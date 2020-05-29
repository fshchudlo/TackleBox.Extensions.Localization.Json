using System;
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TackleBox.Extensions.Localization.Json.JsonLoader
{
    /// <summary>
    /// </summary>
    internal abstract class JsonLoader
    {
        /// <summary>
        /// </summary>
        internal abstract JsonDocument LoadResource(string resourceFileName, CultureInfo? culture);

        internal static JsonLoader Create(IOptions<JsonLocalizationOptions> options, ILogger logger)
        {
            var optionsValue = options.Value.Guard();
            if (optionsValue.ResourceStorageBaseUrl != null)
            {
                return new HttpJsonLoader(optionsValue, logger);
            }

            if (optionsValue.ResourceStorageBaseDirectory != null)
            {
                return new DirectoryJsonLoader(optionsValue, logger);
            }
            throw new InvalidOperationException("Loader type is unknown");
        }
    }
}