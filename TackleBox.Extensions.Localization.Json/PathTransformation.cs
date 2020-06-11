namespace TackleBox.Extensions.Localization.Json
{
    /// <summary>
    /// Specifies whether file name and culture name should be lower-cased or upper-cased
    /// </summary>
    public enum PathTransformation
    {
        /// <summary>
        /// No transformations applied to file name and culture name
        /// </summary>
        None,
        /// <summary>
        /// File name and culture name should be transformed to lower case
        /// </summary>
        LowerCase,
        /// <summary>
        /// File name and culture name should be transformed to lower upper case
        /// </summary>
        UpperCase
    }
}