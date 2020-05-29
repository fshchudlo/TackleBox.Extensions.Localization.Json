using System;
using System.Reflection;
using FluentAssertions;
using TackleBox.Extensions.Localization.Json.Tests.AppSetup;
using Xunit;

// ReSharper disable PossibleNullReferenceException

namespace TackleBox.Extensions.Localization.Json.Tests
{
    public class JsonStringLocalizerFactoryTests
    {
        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldCreateLocalizerFromBasePath(JsonLoaderType loaderType)
        {
            var localizer = CompositionRoot.GetLocalizerFactory(loaderType).Create(TestConstants.ValidResourceFileName, null!) as JsonStringLocalizer;
            localizer.Should().NotBeNull();
            localizer!.ResourceFileName.Should().Be(TestConstants.ValidResourceFileName);
        }

        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldCreateLocalizerFromTypeName(JsonLoaderType loaderType)
        {
            var localizer =
                CompositionRoot.GetLocalizerFactory(loaderType).Create(typeof(LocalizableType)) as JsonStringLocalizer;
            localizer.Should().NotBeNull();
            localizer!.ResourceFileName.Should().Be(typeof(LocalizableType).FullName);
        }

        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldCreateLocalizerBasedOnLocalizedFromAttribute(JsonLoaderType loaderType)
        {
            var localizer =
                CompositionRoot.GetLocalizerFactory(loaderType).Create(typeof(LocalizableTypeWithAttribute)) as
                    JsonStringLocalizer;
            localizer.Should().NotBeNull();
            localizer!.ResourceFileName.Should()
                .Be(typeof(LocalizableTypeWithAttribute).GetCustomAttribute<LocalizedFromAttribute>()!
                    .ResourceFileName);
        }

        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldReturnCachedLocalizerForSameResourceName(JsonLoaderType loaderType)
        {
            var sut = CompositionRoot.GetLocalizerFactory(loaderType);
            var result1 = sut.Create(TestConstants.ValidResourceFileName, null!);
            var result2 = sut.Create(TestConstants.ValidResourceFileName, null!);

            result1.Should().BeSameAs(result2);
        }

        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldReturnCachedLocalizerForSameType(JsonLoaderType loaderType)
        {
            var sut = CompositionRoot.GetLocalizerFactory(loaderType);
            var result1 = sut.Create(typeof(LocalizableTypeWithAttribute));
            var result2 = sut.Create(typeof(LocalizableTypeWithAttribute));
            result1.Should().BeSameAs(result2);
        }

        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldThrowIfPassNullAsType(JsonLoaderType loaderType)
        {
            var sut = CompositionRoot.GetLocalizerFactory(loaderType);
            
            Action act1 = () => sut.Create(null!);
            Action act2 = () => sut.Create(null!, null!);
            
            act1.Should().Throw<ArgumentException>();
            act2.Should().Throw<ArgumentException>();
        }

        public class LocalizableType
        {
        }

        [LocalizedFrom(TestConstants.ValidResourceFileName)]
        public class LocalizableTypeWithAttribute
        {
        }
    }
}