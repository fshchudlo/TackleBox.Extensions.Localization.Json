using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using TackleBox.Extensions.Localization.Json.Tests.AppSetup;
using TackleBox.Extensions.Localization.Json.Tests.HelperTypes;
using Xunit;

namespace TackleBox.Extensions.Localization.Json.Tests
{
    public class AspNetIntegrationTests
    {
        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldConfigureAppToUseHttpJsonStringLocalizerWithoutAnyErrors(JsonLoaderType loaderType)
        {
            var webHostBuilder = new WebHostBuilder()
                .ConfigureServices(services => services.ConfigureLocalization(loaderType))
                .Configure(app =>
                {
                    app.UseRequestLocalization("en-US", "fr-FR");

                    app.Run(context =>
                    {
                        {
                            context.RequestServices.GetService<IStringLocalizer>().Should().BeOfType<JsonStringLocalizer>();
                        }
                        {
                            "en-US".IsCurrentCulture();
                            var localizer = context.RequestServices
                                .GetRequiredService<IStringLocalizer<LocalizableTypeWithAttribute>>();
                            localizer.GetString(x => x.ValueMustEqualOrGreater)
                                .Value.Should()
                                .Be("Value of '{0}' must be greater or equal than '{1}'");
                        }
                        return Task.FromResult(0);
                    });
                });

            using var server = new TestServer(webHostBuilder);
            var client = server.CreateClient();
            Func<Task> act = async () => await client.GetAsync("/");
            act.Should().NotThrow();
        }
    }
}
