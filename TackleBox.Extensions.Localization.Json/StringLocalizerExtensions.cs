using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.Localization;

namespace TackleBox.Extensions.Localization.Json
{
    /// <summary>
    /// </summary>
    public static class StringLocalizerExtensions
    {
        /// <summary>
        /// Returns resource string using member name as a key
        /// </summary>
        public static LocalizedString GetString<TSource>(this IStringLocalizer<TSource> stringLocalizer, Expression<Func<TSource, string>> resourceKeyExpression)
        {
            // ReSharper disable once PossibleNullReferenceException
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return stringLocalizer[(resourceKeyExpression.Body as MemberExpression).Member.Name];
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        /// <summary>
        /// Returns resource string using member name as a key
        /// </summary>
        public static LocalizedString GetString<TSource>(this IStringLocalizer<TSource> stringLocalizer, Expression<Func<TSource, string>> resourceKeyExpression, params object[] arguments)
        {
            // ReSharper disable once PossibleNullReferenceException
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return stringLocalizer[(resourceKeyExpression.Body as MemberExpression).Member.Name, arguments];
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        /// <summary>
        /// Returns resource string using member name as a key
        /// </summary>
        public static LocalizedString FormatLocalized(this IStringLocalizer stringLocalizer, string resourceKey, params string[] formatKeys)
        {
            LocalizedString formattedMessage;
            if (formatKeys != null && formatKeys.Any())
            {
                // ReSharper disable once CoVariantArrayConversion
                formattedMessage = stringLocalizer[resourceKey, formatKeys.Select(f => stringLocalizer[f].Value).ToArray()];
            }
            else
            {
                formattedMessage = stringLocalizer[resourceKey];
            }

            return formattedMessage;
        }
        /// <summary>
        /// Returns resource string using member name as a key
        /// </summary>
        public static LocalizedString FormatLocalized<TSource>(this IStringLocalizer<TSource> stringLocalizer, Expression<Func<TSource, string>> resourceKeyExpression, params string[] formatKeys)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            // ReSharper disable once PossibleNullReferenceException
            var resourceKey = (resourceKeyExpression.Body as MemberExpression).Member.Name;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            return FormatLocalized(stringLocalizer, resourceKey, formatKeys);
        }

        /// <summary>
        /// Returns resource string or throws if it wasn't found
        /// </summary>
        /// <exception cref="KeyNotFoundException"></exception>
        public static LocalizedString GetRequiredString(this IStringLocalizer stringLocalizer, string resourceKey)
        {
            var result = stringLocalizer[resourceKey];
            if (result.ResourceNotFound)
            {
                var message = $"Translation of '{resourceKey}' key wasn't found at.";
                throw new KeyNotFoundException(message);
            }
            return result;
        }

        /// <summary>
        /// Returns resource string or throws if it wasn't found
        /// </summary>
        /// <exception cref="KeyNotFoundException"></exception>
        public static LocalizedString GetRequiredString<TSource>(this IStringLocalizer<TSource> stringLocalizer, Expression<Func<TSource, string>> resourceKeyExpression, params string[] formatKeys)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            // ReSharper disable once PossibleNullReferenceException
            var resourceKey = (resourceKeyExpression.Body as MemberExpression).Member.Name;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            return GetRequiredString(stringLocalizer, resourceKey);
        }

        /// <summary>
        /// Checks that resource key exists for all members of <typeparamref name="TSource"/> type.
        /// Throws <see cref="ResourceIntegrityException"/> otherwise.
        /// </summary>
        /// <param name="stringLocalizer"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <exception cref="ResourceIntegrityException"></exception>
        public static void ValidateResourceIntegrity<TSource>(this IStringLocalizer<TSource> stringLocalizer)
        {
            var sourceKeys = 
                typeof(TSource).GetProperties().Where(pi => pi.CanRead && pi.GetMethod.IsPublic).Select(p => p.Name)
                    .ToArray()
                    .Concat(typeof(TSource).GetFields().Where(fi => fi.IsPublic).Select(p => p.Name));
            var resourceKeys = stringLocalizer.GetAllStrings(true).Select(ls => ls.Name);
            var missedString = sourceKeys.Except(resourceKeys).ToArray();
            if (missedString.Any())
            {
                throw new ResourceIntegrityException(typeof(TSource), missedString);
            }
        }
    }
}