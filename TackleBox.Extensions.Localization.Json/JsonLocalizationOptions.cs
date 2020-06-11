using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace TackleBox.Extensions.Localization.Json
{
    /// <summary>
    /// </summary>
    public class JsonLocalizationOptions
    {
        internal string? ResourceStorageBaseUrl { get; private set; }
        internal string? ResourceStorageBaseDirectory { get; private set; }

        /// <summary>
        /// When true, no exceptions will be thrown on resource loading errors which allows to work even when CDN with resources is unreachable.
        /// Default is <value>false</value>
        /// </summary>
        public bool UseSilentDegradation { get; set; } = false;

        /// <summary>
        /// If true, <see cref="KeyNotFoundException"/> will be thrown when the specified key does not match any key in the resource file.
        /// Default is <value>false</value>
        /// </summary>
        public bool ThrowIfKeyDoesNotExist { get; set; } = false;

        /// <summary>
        /// Sets culture to use if it couldn't be detected from current thread
        /// Default is <value>null</value>
        /// </summary>
        public CultureInfo? DefaultCulture { get; set; } = null;

        /// <summary>
        /// Default is <value>30 minutes</value>
        /// </summary>
        public TimeSpan CacheExpirationLimit { get; set; } = TimeSpan.FromMinutes(30);

        internal Func<JsonLocalizationOptions, string, CultureInfo, string> ResourcePathBuilder;

        /// <summary>
        /// Configures IStringLocalizer to load json files via http requests from the specified baseUrl
        /// </summary>
        /// <param name="baseUrl">Base address of the server (typically is a CDN) to load json resource files</param>
        /// <param name="resourceUrlBuilder">Allows to specify custom way to build url to resource file</param>
        public JsonLocalizationOptions FromHttp(string baseUrl,
            Func<JsonLocalizationOptions, string, CultureInfo, string>? resourceUrlBuilder = null)
        {
            ResourceStorageBaseUrl = baseUrl;
            ResourcePathBuilder = resourceUrlBuilder ?? ((options, resourceFileName, culture) =>
            {
                var normalizedBaseUrl = options.ResourceStorageBaseUrl!.ToString().EndsWith("/")
                    ? options.ResourceStorageBaseUrl.ToString()
                    : $"{options.ResourceStorageBaseUrl}/";
                
                var filePath = resourceFileName == string.Empty
                    ? $"{culture.Name.ToLower()}.json"
                    : $"{culture.Name.ToLower()}/{resourceFileName.ToLower()}.json";
                
                return $"{normalizedBaseUrl}{filePath}";
            });
            return this;
        }

        /// <summary>
        /// Configures IStringLocalizer to load json files from directory specified with basePath
        /// </summary>
        /// <param name="basePath">Base directory to load json resource files</param>
        /// <param name="resourcePathBuilder">Allows to specify custom way to build path to resource file</param>
        public JsonLocalizationOptions FromDirectory(string basePath,
            Func<JsonLocalizationOptions, string, CultureInfo, string>? resourcePathBuilder = null)
        {
            ResourceStorageBaseDirectory = basePath;
            ResourcePathBuilder = resourcePathBuilder ?? ((options, resourceFileName, culture) =>
                resourceFileName == string.Empty
                    ? Path.Combine(options.ResourceStorageBaseDirectory, $"{culture.Name}.json")
                    : Path.Combine(options.ResourceStorageBaseDirectory, culture.Name,
                        $"{resourceFileName}.json"));
            return this;
        }

        internal JsonLocalizationOptions Guard()
        {
            CheckExactlyOneIsConfigured(ResourceStorageBaseUrl, ResourceStorageBaseDirectory);
            return this;
        }

        private static void CheckExactlyOneIsConfigured(params string?[] values)
        {
            var configuredCount = values.Count(v => !string.IsNullOrWhiteSpace(v));
            if (configuredCount == 0)
            {
                throw new InvalidOperationException(
                    $"JsonStringLocalizer should be configured with either {nameof(FromHttp)} or {nameof(FromDirectory)} method");
            }

            if (configuredCount > 1)
            {
                throw new InvalidOperationException(
                    $"JsonStringLocalizer should be configured with only one of the following methods: {nameof(FromHttp)}, {nameof(FromDirectory)}");
            }
        }
    }
}