using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Localization;
using TackleBox.Extensions.Localization.Json.JsonLoader;
using TackleBox.Extensions.Localization.Json.Tests.AppSetup;
using TackleBox.Extensions.Localization.Json.Tests.HelperTypes;
using Xunit;

// ReSharper disable StringLiteralTypo

namespace TackleBox.Extensions.Localization.Json.Tests
{
    public class JsonStringLocalizerTests
    {
        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldCreateNonGenericLocalizerFromFileWithCultureName(JsonLoaderType loaderType)
        {
            "en-US".IsCurrentCulture();
            var localizer = CompositionRoot.GetLocalizer(loaderType);
            localizer.Should().NotBeNull();
            localizer["RootKey"].Value.Should().Be("Root resource file key");
        }

        [Theory]
        [InlineData(JsonLoaderType.Directory, "en-US", TestConstants.ValidResourceKey,
            "Value of '{0}' must be greater or equal than '{1}'")]
        [InlineData(JsonLoaderType.Directory, "fr-FR", TestConstants.ValidResourceKey,
            "La valeur de '{0}' doit être supérieure ou égale à '{1}'")]
        [InlineData(JsonLoaderType.Http, "en-US", TestConstants.ValidResourceKey,
            "Value of '{0}' must be greater or equal than '{1}'")]
        [InlineData(JsonLoaderType.Http, "fr-FR", TestConstants.ValidResourceKey,
            "La valeur de '{0}' doit être supérieure ou égale à '{1}'")]
        public void ShouldReturnValues(JsonLoaderType loaderType, string cultureName, string resourceKey, string expectedValue)
        {
            cultureName.IsCurrentCulture();
            var sut = CompositionRoot.GetLocalizer<LocalizableTypeWithAttribute>(loaderType);
            var translation = sut[resourceKey];
            translation.Value.Should().Be(expectedValue);
        }


        [Theory]
        [InlineData(JsonLoaderType.Directory, "en-US", TestConstants.ValidResourceKey,
            "Value of 'FieldName' must be greater or equal than 'FieldValue'", "FieldName", "FieldValue")]
        [InlineData(JsonLoaderType.Directory, "fr-FR", TestConstants.ValidResourceKey,
            "La valeur de 'Nom de domaine' doit être supérieure ou égale à 'Valeur du champ'", "Nom de domaine",
            "Valeur du champ")]
        [InlineData(JsonLoaderType.Http, "en-US", TestConstants.ValidResourceKey,
            "Value of 'FieldName' must be greater or equal than 'FieldValue'", "FieldName", "FieldValue")]
        [InlineData(JsonLoaderType.Http, "fr-FR", TestConstants.ValidResourceKey,
            "La valeur de 'Nom de domaine' doit être supérieure ou égale à 'Valeur du champ'", "Nom de domaine",
            "Valeur du champ")]
        public void ShouldReturnFormattedValues(JsonLoaderType loaderType, string cultureName, string resourceKey,
            string expectedValue, string fieldName, string fieldValue)
        {
            cultureName.IsCurrentCulture();
            var sut = CompositionRoot.GetLocalizer<LocalizableTypeWithAttribute>(loaderType);
            var translation = sut[resourceKey, fieldName, fieldValue];
            translation.Value.Should().Be(expectedValue);
        }

        [Theory]
        [InlineData(JsonLoaderType.Directory, true, 3)]
        [InlineData(JsonLoaderType.Directory, false, 3)]
        [InlineData(JsonLoaderType.Http, true, 3)]
        [InlineData(JsonLoaderType.Http, false, 3)]
        public void ShouldReturnAllStrings(JsonLoaderType loaderType, bool includeParent, int expectedCount)
        {
            "en-US".IsCurrentCulture();
            var sut = CompositionRoot.GetLocalizer<LocalizableTypeWithAttribute>(loaderType);
            var localizedStrings = sut.GetAllStrings(includeParent);

            localizedStrings.Count().Should().Be(expectedCount);
        }

        [Theory]
        [InlineData(JsonLoaderType.Directory, "en-US",
            "Value of 'FieldName' must be greater or equal than 'FieldValue'", "FieldName", "FieldValue")]
        [InlineData(JsonLoaderType.Directory, "fr-FR",
            "La valeur de 'Nom de Champ' doit être supérieure ou égale à 'Valeur de Champ'", "Nom de Champ",
            "Valeur de Champ")]
        [InlineData(JsonLoaderType.Http, "en-US", "Value of 'FieldName' must be greater or equal than 'FieldValue'",
            "FieldName", "FieldValue")]
        [InlineData(JsonLoaderType.Http, "fr-FR",
            "La valeur de 'Nom de Champ' doit être supérieure ou égale à 'Valeur de Champ'", "Nom de Champ",
            "Valeur de Champ")]
        public void ShouldReturnValueWithStronglyTypedOverload(JsonLoaderType loaderType, string cultureName,
            string expectedValue, string fieldName, string fieldValue)
        {
            cultureName.IsCurrentCulture();
            var sut = CompositionRoot.GetLocalizer<LocalizableTypeWithAttribute>(loaderType);

            var translation = sut.GetString(r => r.ValueMustEqualOrGreater, fieldName, fieldValue);
            
            translation.Value.Should().Be(expectedValue);
        }

        [Theory]
        [InlineData(JsonLoaderType.Directory, "en-US",
            "Value of 'Field Name' must be greater or equal than 'Field Value'")]
        [InlineData(JsonLoaderType.Directory, "fr-FR",
            "La valeur de 'Nom de Champ' doit être supérieure ou égale à 'Valeur de Champ'")]
        [InlineData(JsonLoaderType.Http, "en-US", "Value of 'Field Name' must be greater or equal than 'Field Value'")]
        [InlineData(JsonLoaderType.Http, "fr-FR",
            "La valeur de 'Nom de Champ' doit être supérieure ou égale à 'Valeur de Champ'")]
        public void ShouldReturnLocalizedFormattedValue(JsonLoaderType loaderType, string cultureName,
            string expectedValue)
        {
            cultureName.IsCurrentCulture();
            var sut = CompositionRoot.GetLocalizer<LocalizableTypeWithAttribute>(loaderType);
            
            var translation = sut.FormatLocalized("ValueMustEqualOrGreater", "FieldName", "FieldValue");

            translation.Value.Should().Be(expectedValue);
            translation = sut.FormatLocalized(r => r.ValueMustEqualOrGreater, "FieldName", "FieldValue");
            translation.Value.Should().Be(expectedValue);
        }

        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldReturnValueIfFormatLocalizedIsCalledWithoutArguments(JsonLoaderType loaderType)
        {
            "en-US".IsCurrentCulture();
            var sut = CompositionRoot.GetLocalizer<LocalizableTypeWithAttribute>(loaderType);

            var translation = sut.FormatLocalized("ValueMustEqualOrGreater");

            translation.Value.Should().Be(sut["ValueMustEqualOrGreater"].Value);
            translation = sut.FormatLocalized(r => r.ValueMustEqualOrGreater);
            translation.Value.Should().Be(sut["ValueMustEqualOrGreater"].Value);
        }

        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void GetRequiredStringShouldThrowIfKeyNotFound(JsonLoaderType loaderType)
        {
            "en-US".IsCurrentCulture();
            var sut = CompositionRoot.GetLocalizer<LocalizableTypeWithAttribute>(loaderType);

            Action validAct1 = () => sut.GetRequiredString("ValueMustEqualOrGreater");
            validAct1.Should().NotThrow();

            Action validAct2 = () => sut.GetRequiredString(x => x.ValueMustEqualOrGreater);
            validAct2.Should().NotThrow();

            Action invalidAct = () => sut.GetRequiredString(TestConstants.InvalidResourceKey);

            invalidAct.Should().Throw<KeyNotFoundException>();
        }

        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldReturnPassedKeyIfThereIsNoValue(JsonLoaderType loaderType)
        {
            "en-US".IsCurrentCulture();
            var sut = CompositionRoot.GetLocalizer<LocalizableTypeWithAttribute>(loaderType);

            var result = sut[TestConstants.InvalidResourceKey];
            
            result.Value.Should().Be(TestConstants.InvalidResourceKey);
            result.ResourceNotFound.Should().BeTrue();
        }

        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldThrowOnMissedResourceFile(JsonLoaderType loaderType)
        {
            "en-US".IsCurrentCulture();
            var sut = CompositionRoot.GetLocalizer<JsonStringLocalizerTests>(loaderType);
            Func<string> act = () => sut[TestConstants.ValidResourceKey];

            var assertion = act.Should()
                .Throw<UnableToLoadResourceFileException>();

            assertion.Which.CultureName.Should().Be("en-US");
            assertion.Which.ResourceFileName.Should().Be(typeof(JsonStringLocalizerTests).FullName);
            switch (loaderType)
            {
                case JsonLoaderType.Http:
                    assertion.Which.InnerException.Should().BeOfType<WebException>();
                    break;
                case JsonLoaderType.Directory:
                    assertion.Which.InnerException.Should().BeOfType<FileNotFoundException>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(loaderType), loaderType, null);
            }
        }

        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldThrowIfPassNullAsKey(JsonLoaderType loaderType)
        {
            "en-US".IsCurrentCulture();
            var sut = CompositionRoot.GetLocalizer<JsonStringLocalizerTests>(loaderType);
            
            Func<string> act1 = () => sut[null];
            Func<string> act2 = () => sut.GetString(null as string);
            Func<string> act3 = () => sut.GetString(null as string, null!);

            act1.Should().Throw<ArgumentNullException>();
            act2.Should().Throw<ArgumentNullException>();
            act3.Should().Throw<ArgumentNullException>();
        }

        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ShouldReturnStringLocalizerForSpecifiedCulture(JsonLoaderType loaderType)
        {
            "en-US".IsCurrentCulture();
            var sut = CompositionRoot.GetLocalizer<LocalizableTypeWithAttribute>(loaderType);
            
            sut
                .GetString("ValueMustEqualOrGreater").Value
                .Should().Be("Value of '{0}' must be greater or equal than '{1}'");

#pragma warning disable 618
            sut.WithCulture(new CultureInfo("fr-FR"))
#pragma warning restore 618
                .GetString("ValueMustEqualOrGreater").Value
                .Should().Be("La valeur de '{0}' doit être supérieure ou égale à '{1}'");
        }

        [Theory, InlineData(JsonLoaderType.Http), InlineData(JsonLoaderType.Directory)]
        public void ValidateResourceIntegrityShouldThrowIfSomeKeysAreMissed(JsonLoaderType loaderType)
        {
            "en-US".IsCurrentCulture();
            Action validAct = () =>
                CompositionRoot.GetLocalizer<LocalizableTypeWithAttribute>(loaderType).ValidateResourceIntegrity();
            validAct.Should().NotThrow();

            Action invalidAct = () =>
                CompositionRoot.GetLocalizer<LocalizableTypeWithMissedKey>(loaderType).ValidateResourceIntegrity();

            var assertion = invalidAct.Should().Throw<ResourceIntegrityException>();
            assertion.Which.MissedStrings.Should()
                .BeEquivalentTo(nameof(LocalizableTypeWithMissedKey.InvalidPropertyForIntegrityValidation));
            assertion.Which.CheckedType.Should().Be(typeof(LocalizableTypeWithMissedKey));
        }
    }
}