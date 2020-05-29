using System;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.Localization;
using TackleBox.Extensions.Localization.Json.JsonLoader;
using TackleBox.Extensions.Localization.Json.StringFormatter;

namespace TackleBox.Extensions.Localization.Json
{
    /// <inheritdoc />
    internal class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly JsonResourceProvider _resourceProvider;
        private readonly IStringFormatter _stringFormatter;
        private readonly ConcurrentDictionary<string, JsonStringLocalizer> _localizerCache = new ConcurrentDictionary<string, JsonStringLocalizer>();

        /// <summary>
        /// </summary>
        public JsonStringLocalizerFactory(JsonResourceProvider resourceProvider, IStringFormatter stringFormatter)
        {
            _resourceProvider = resourceProvider ?? throw new ArgumentNullException(nameof(resourceProvider));
            _stringFormatter = stringFormatter ?? throw new ArgumentNullException(nameof(stringFormatter));
        }

        /// <summary>
        /// Creates a <see cref="JsonStringLocalizer"/> using the <see cref="LocalizedFromAttribute"/>.
        /// If type is not decorated with <see cref="LocalizedFromAttribute"/> then <see cref="Type.FullName"/> is used as a name of resource file.
        /// </summary>
        /// <param name="resourceSource">The <see cref="Type"/>.</param>
        /// <returns>The <see cref="JsonStringLocalizer"/>.</returns>
        public IStringLocalizer Create(Type resourceSource)
        {
            if (resourceSource == null)
            {
                throw new ArgumentNullException(nameof(resourceSource));
            }

            var typeInfo = resourceSource.GetTypeInfo();
            var resourceFileNameAttribute = typeInfo.GetCustomAttribute<LocalizedFromAttribute>() ?? typeInfo.Assembly.GetCustomAttribute<LocalizedFromAttribute>();
            var baseName = resourceFileNameAttribute?.ResourceFileName ?? typeInfo.FullName;
            return _localizerCache.GetOrAdd(baseName, _ => CreateJsonStringLocalizer(baseName));
        }

        /// <summary>
        /// Creates a <see cref="JsonStringLocalizer"/>.
        /// </summary>
        /// <param name="resourceFileName">The name of the resource file to load strings from.</param>
        /// <param name="location">This parameter is useless and it's here just to implement <see cref="IStringLocalizerFactory"/>.</param>
        /// <returns>The <see cref="JsonStringLocalizer"/>.</returns>
        public IStringLocalizer Create(string resourceFileName, string location)
        {
            if (resourceFileName == null)
            {
                throw new ArgumentNullException(nameof(resourceFileName));
            }
            return _localizerCache.GetOrAdd(resourceFileName, _ => CreateJsonStringLocalizer(resourceFileName));
        }

        private JsonStringLocalizer CreateJsonStringLocalizer(string resourceFileName)
        {
            return new JsonStringLocalizer(resourceFileName, _resourceProvider, _stringFormatter);
        }
    }
}
