using System.Globalization;

namespace TackleBox.Extensions.Localization.Json.Tests.AppSetup
{
    public static class TestExtensions
    {
        public static void IsCurrentCulture(this string cultureName)
        {
            var culture = new CultureInfo(cultureName);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
    }
}
