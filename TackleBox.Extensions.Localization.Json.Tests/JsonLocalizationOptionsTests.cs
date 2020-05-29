using System;
using FluentAssertions;
using Xunit;

namespace TackleBox.Extensions.Localization.Json.Tests
{
    public class JsonLocalizationOptionsTests
    {
        [Fact]
        public void ShouldNotThrowIfLoaderConfiguredCorrectly()
        {
            var sut = new JsonLocalizationOptions().FromHttp("http://test.com");
            
            Action act = () => sut.Guard();
            
            act.Should().NotThrow();
        }

        [Fact]
        public void ShouldThrowIfLoaderIsNotConfigured()
        {
            var sut = new JsonLocalizationOptions();
            
            Action act = () => sut.Guard();
            
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("JsonStringLocalizer should be configured with either*");
        }
        [Fact]
        public void ShouldThrowIfLoaderIsConfiguredForSeveralSources()
        {
            var sut = new JsonLocalizationOptions().FromHttp("http://test.com").FromDirectory("/var");
            
            Action act = () => sut.Guard();
            
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("JsonStringLocalizer should be configured with only one*");
        }
    }
}