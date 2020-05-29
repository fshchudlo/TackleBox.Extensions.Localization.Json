using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;
using TackleBox.Extensions.Localization.Json.JsonLoader;
using TackleBox.Extensions.Localization.Json.StringFormatter;

namespace TackleBox.Extensions.Localization.Json
{
    /// <inheritdoc />
    internal sealed class JsonStringLocalizer : IStringLocalizer
    {
        /// <summary>
        /// </summary>
        public readonly string ResourceFileName;
        private readonly ConcurrentDictionary<string, object> _missingStringsCache = new ConcurrentDictionary<string, object>();
        private readonly IStringFormatter _stringFormatter;
        private readonly Func<CultureInfo> _getCulture;
        private readonly JsonResourceProvider _resourceProvider;

        /// <summary>
        /// </summary>
        public JsonStringLocalizer(string resourceFileName, JsonResourceProvider resourceProvider, IStringFormatter stringFormatter)
        {
            ResourceFileName = resourceFileName ?? throw new ArgumentNullException(nameof(resourceFileName));
            _resourceProvider = resourceProvider;
            _stringFormatter = stringFormatter;
            _getCulture = () => CultureInfo.CurrentUICulture;
        }

        private JsonStringLocalizer(JsonStringLocalizer baseLocalizer, CultureInfo culture)
        {
            ResourceFileName = baseLocalizer.ResourceFileName;
            _resourceProvider = baseLocalizer._resourceProvider;
            _stringFormatter = baseLocalizer._stringFormatter;
            _getCulture = () => culture;
        }

        /// <summary>
        /// Returns new localizer instance which is configured for specified culture
        /// </summary>
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new JsonStringLocalizer(this, culture);
        }

        /// <inheritdoc />
        public LocalizedString this[string resourceKey]
        {
            get
            {
                var value = GetStringInternal(resourceKey, null);

                return new LocalizedString(resourceKey, value ?? resourceKey, value == null, ResourceFileName);
            }
        }

        /// <inheritdoc />
        public LocalizedString this[string resourceKey, params object[] arguments]
        {
            get
            {
                var format = GetStringInternal(resourceKey, null);
                var value = _stringFormatter.Format(format ?? resourceKey, arguments);

                return new LocalizedString(resourceKey, value, format == null, ResourceFileName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="includeParentCultures">This parameter is ignored since json files always contain full set of strings</param>
        /// <returns></returns>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var resourceKeys = _resourceProvider.GetAllResourceKeys(ResourceFileName, _getCulture());

            foreach (var resourceKey in resourceKeys)
            {
                var value = GetStringInternal(resourceKey, _getCulture());
                yield return new LocalizedString(resourceKey, value ?? resourceKey, value == null, ResourceFileName);
            }
        }

        private string? GetStringInternal(string resourceKey, CultureInfo? culture)
        {
            if (resourceKey == null)
            {
                throw new ArgumentNullException(nameof(resourceKey));
            }

            var keyCulture = culture ?? _getCulture();

            var cacheKey = $"name={resourceKey}&culture={keyCulture.Name}";

            if (_missingStringsCache.ContainsKey(cacheKey))
            {
                return null;
            }

            var value = _resourceProvider.GetTranslation(ResourceFileName, resourceKey, culture ?? _getCulture());
            if (value == null)
            {
                _missingStringsCache.TryAdd(cacheKey, null!);
            }

            return value;
        }
    }
}
