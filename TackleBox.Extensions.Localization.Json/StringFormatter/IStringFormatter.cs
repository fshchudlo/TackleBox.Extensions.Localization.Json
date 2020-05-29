namespace TackleBox.Extensions.Localization.Json.StringFormatter
{
    /// <summary>
    /// Contract for resource string formatting.
    /// For example, it can replace standard string.Format with Intl format syntax and so on
    /// </summary>
    public interface IStringFormatter
    {
        /// <summary>
        /// </summary>
        string Format(string format, params object?[] args);
    }
}
