using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace TackleBox.Extensions.Localization.Json.Tests.AppSetup
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureLocalization(this IServiceCollection serviceCollection, JsonLoaderType loaderType)
        {
            serviceCollection.AddSingleton<ILoggerFactory, NullLoggerFactory>();
            switch (loaderType)
            {
                case JsonLoaderType.Http:
                    serviceCollection.AddJsonLocalization(options =>
                    {
                        options.FromHttp("https://raw.githubusercontent.com/fshchudlo/TackleBox.Extensions.Localization.Json/master/TackleBox.Extensions.Localization.Json.Tests/TestJsonResources/");
                    });
                    break;
                case JsonLoaderType.Directory:
                    serviceCollection.AddJsonLocalization(options =>
                    {
                        var executionDir = Path.GetDirectoryName(typeof(CompositionRoot).Assembly.Location);
                        options.FromDirectory(Path.Combine(executionDir!, "TestJsonResources"));
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(loaderType), loaderType, null);
            }
            return serviceCollection;
        }
    }
}
