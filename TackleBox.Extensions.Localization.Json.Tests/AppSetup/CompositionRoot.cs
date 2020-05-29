using System;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using TackleBox.Extensions.Localization.Json.JsonLoader;

namespace TackleBox.Extensions.Localization.Json.Tests.AppSetup
{
    internal static class CompositionRoot
    {
        public static IStringLocalizer GetLocalizer(JsonLoaderType loaderType)
        {
            return GetServiceCollection(loaderType)
                .BuildServiceProvider()
                .GetRequiredService<IStringLocalizer>();
        }
        public static IStringLocalizer<T> GetLocalizer<T>(JsonLoaderType loaderType)
        {
            return GetServiceCollection(loaderType)
                .BuildServiceProvider()
                .GetRequiredService<IStringLocalizer<T>>();
        }
        public static JsonStringLocalizerFactory GetLocalizerFactory(JsonLoaderType loaderType)
        {
            if (GetServiceCollection(loaderType).BuildServiceProvider().GetRequiredService<IStringLocalizerFactory>() is JsonStringLocalizerFactory service)
            {
                return service;
            }
            throw new InvalidOperationException($"{nameof(IStringLocalizerFactory)} is not {nameof(JsonStringLocalizerFactory)}. Localization configuration is invalid.");
        }
        public static JsonResourceProvider GetResourceProvider(JsonLoaderType loaderType)
        {
            return GetServiceCollection(loaderType)
                .BuildServiceProvider()
                .GetRequiredService<JsonResourceProvider>();
        }
        public static JsonResourceProvider GetResourceProviderWithSilentDegradation(JsonLoaderType loaderType)
        {
            return GetServiceCollection(loaderType)
                .Configure<JsonLocalizationOptions>(options => { options.UseSilentDegradation = true; })
                .BuildServiceProvider()
                .GetRequiredService<JsonResourceProvider>();
        }
        public static JsonResourceProvider GetResourceProviderWithDefaultCulture(JsonLoaderType loaderType, CultureInfo? value)
        {
            return GetServiceCollection(loaderType)
                .Configure<JsonLocalizationOptions>(options => { options.DefaultCulture = value; })
                .BuildServiceProvider()
                .GetRequiredService<JsonResourceProvider>();
        }
        public static JsonResourceProvider GetResourceProviderThatThrowsIfTranslationKeyDoesNotExist(JsonLoaderType loaderType)
        {
            return GetServiceCollection(loaderType)
                .Configure<JsonLocalizationOptions>(options => { options.ThrowIfKeyDoesNotExist = true; })
                .BuildServiceProvider()
                .GetRequiredService<JsonResourceProvider>();
        }

        private static IServiceCollection GetServiceCollection(JsonLoaderType loaderType)
        {
            return new ServiceCollection().ConfigureLocalization(loaderType);
        }
    }
}