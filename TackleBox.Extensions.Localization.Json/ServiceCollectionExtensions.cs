using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using TackleBox.Extensions.Localization.Json.JsonLoader;
using TackleBox.Extensions.Localization.Json.StringFormatter;

// ReSharper disable UnusedMember.Global

namespace TackleBox.Extensions.Localization.Json
{
    /// <summary>
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// </summary>
        public static IServiceCollection AddJsonLocalization(this IServiceCollection services,
            Action<JsonLocalizationOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.TryAddSingleton<IStringFormatter, StringFormatStringFormatter>();
            services.TryAddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            services.TryAddSingleton<JsonResourceProvider>();
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
            services.TryAddTransient(isp=>isp.GetRequiredService<IStringLocalizerFactory>().Create(string.Empty, null));
            services.Configure(setupAction);
            return services;
        }
    }
}