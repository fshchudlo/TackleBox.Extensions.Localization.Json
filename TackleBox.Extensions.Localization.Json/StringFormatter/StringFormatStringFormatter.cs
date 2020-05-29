namespace TackleBox.Extensions.Localization.Json.StringFormatter
{
    /// <summary>
    /// Implementation of <see cref="IStringFormatter"/> based on String.Format syntax
    /// </summary>
    public class StringFormatStringFormatter : IStringFormatter
    {
        /// <inheritdoc />
        public string Format(string format, params object?[] args)
        {
            return string.Format(format, args);
        }
    }
}