using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using TackleBox.Extensions.Localization.Json.JsonLoader;
using TackleBox.Extensions.Localization.Json.Tests.AppSetup;
using Xunit;

namespace TackleBox.Extensions.Localization.Json.Tests
{
    public class JsonResourceProviderTests
    {
        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldLoadJsonFromConfiguredSource(JsonLoaderType loaderType)
        {
            var sut = CompositionRoot.GetResourceProvider(loaderType);
            var translation = sut.GetTranslation(TestConstants.ValidResourceFileName, "ValueMustEqualOrGreater", new CultureInfo("fr-FR"));
            translation.Should().NotBeNull();
            // ReSharper disable StringLiteralTypo
            translation.Should().Be("La valeur de '{0}' doit être supérieure ou égale à '{1}'");
            // ReSharper restore StringLiteralTypo
        }

        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldThrowOnInvalidResourceFileName(JsonLoaderType loaderType)
        {
            var sut = CompositionRoot.GetResourceProvider(loaderType);
            Action act = () => sut.GetAllResourceKeys(TestConstants.InvalidResourceFileName, new CultureInfo("fr-FR"));

            var assertion = act.Should()
                .Throw<UnableToLoadResourceFileException>();

            assertion.Which.CultureName.Should().Be("fr-FR");
            assertion.Which.ResourceFileName.Should().Be(TestConstants.InvalidResourceFileName);
        }

        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldThrowIfResourceFileNotFoundForSpecifiedCulture(JsonLoaderType loaderType)
        {
            var sut = CompositionRoot.GetResourceProvider(loaderType);
            Action act = () => sut.GetAllResourceKeys(TestConstants.ValidResourceFileName, new CultureInfo("ja-JP"));

            var assertion = act.Should()
                .Throw<UnableToLoadResourceFileException>();

            assertion.Which.CultureName.Should().Be("ja-JP");
            assertion.Which.ResourceFileName.Should().Be(TestConstants.ValidResourceFileName);
        }
        
        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldTreatNullAsEmptyCultureAndTryToLoadIt(JsonLoaderType loaderType)
        {
            var sut = CompositionRoot.GetResourceProvider(loaderType);
            Action act = () => sut.GetAllResourceKeys(TestConstants.ValidResourceFileName, null!);

            var assertion = act.Should()
                .Throw<UnableToLoadResourceFileException>();

            assertion.Which.CultureName.Should().Be(string.Empty);
            assertion.Which.ResourceFileName.Should().Be(TestConstants.ValidResourceFileName);
        }

        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldNotThrowIfUseSilentDegradationFlagIsSetToTrue(JsonLoaderType loaderType)
        {
            var sut = CompositionRoot.GetResourceProviderWithSilentDegradation(loaderType);
            
            Action act = () => sut.GetAllResourceKeys(TestConstants.ValidResourceFileName, new CultureInfo("ja-JP"));
            act.Should().NotThrow();
        }

        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldThrowIfKeyNotFoundAndThrowIfKeyDoesNotExistIsSetToTrue(JsonLoaderType loaderType)
        {
            var sut = CompositionRoot.GetResourceProviderThatThrowsIfTranslationKeyDoesNotExist(loaderType);

            Func<string?> act = () => sut.GetTranslation(TestConstants.ValidResourceFileName, "DefinitelyInvalidResourceKey", new CultureInfo("fr-FR"));
            act.Should().Throw<KeyNotFoundException>();
        }
        
        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldNotThrowByDefaultIfKeyNotFound(JsonLoaderType loaderType)
        {
            var sut = CompositionRoot.GetResourceProvider(loaderType);

            Func<string?> act = () => sut.GetTranslation(TestConstants.ValidResourceFileName, "DefinitelyInvalidResourceKey", new CultureInfo("fr-FR"));
            act.Should().NotThrow();
            act().Should().BeNull();
        }

        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldUseConfiguredDefaultCultureIfCurrentCultureIsEmpty(JsonLoaderType loaderType)
        {
            var sut = CompositionRoot.GetResourceProviderWithDefaultCulture(loaderType, new CultureInfo("en-US"));
            Action act = () => sut.GetAllResourceKeys(TestConstants.ValidResourceFileName, new CultureInfo(""));
            act.Should().NotThrow();
        }
    }
}
