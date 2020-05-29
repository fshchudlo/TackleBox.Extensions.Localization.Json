using JetBrains.Annotations;

namespace TackleBox.Extensions.Localization.Json.Tests.HelperTypes
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    [LocalizedFrom(TestConstants.ValidResourceFileName)]
    public class LocalizableTypeWithAttribute
    {
        public string ValueMustEqualOrGreater;
        public string FieldName;
        public string FieldValue;
    }
}