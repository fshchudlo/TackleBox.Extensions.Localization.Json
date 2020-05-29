using JetBrains.Annotations;

namespace TackleBox.Extensions.Localization.Json.Tests.HelperTypes
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    [LocalizedFrom(TestConstants.ValidResourceFileName)]
    public class LocalizableTypeWithMissedKey
    {
        public string ValueMustEqualOrGreater { get; set; }
        public string FieldName;
        public string FieldValue;
        
        public string InvalidPropertyForIntegrityValidation { get; set; }
        public string PrivatePropertyForIntegrityValidation { private get; set; }
#pragma warning disable 8618
        private string _privateFieldForIntegrityValidation;
#pragma warning restore 8618
        public void MethodForIntegrityValidation()
        {

        }
    }
}