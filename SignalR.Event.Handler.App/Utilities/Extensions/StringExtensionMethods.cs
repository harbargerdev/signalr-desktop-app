using System;

namespace SignalR.Event.Handler.App.Utilities.Extensions
{
    public static class StringExtensionMethods
    {
        /// <summary>
        /// Determines whether the specified string is null, empty, or consists only of white-space characters after
        /// trimming.
        /// </summary>
        /// <remarks>This method trims the input string before evaluating whether it is empty. It is
        /// useful for validating user input where white-space should be treated as empty.</remarks>
        /// <param name="str">The string to test for null, empty, or white-space content.</param>
        /// <returns>true if the string is null, empty, or contains only white-space characters; otherwise, false.</returns>
        public static bool IsNullOrEmptyTrimmed(this string str)
        {
            return string.IsNullOrEmpty(str?.Trim());
        }

        /// <summary>
        /// Determines whether two strings are equal, using an ordinal, case-insensitive comparison.
        /// </summary>
        /// <remarks>Comparison is performed using <see cref="StringComparison.OrdinalIgnoreCase"/>. Null
        /// values are considered equal.</remarks>
        /// <param name="str">The first string to compare.</param>
        /// <param name="other">The second string to compare to <paramref name="str"/>.</param>
        /// <returns>true if the strings are equal when compared using ordinal, case-insensitive rules; otherwise, false.</returns>
        public static bool CompareIgnoreCase(this string str, string other)
        {
            return string.Equals(str, other, StringComparison.OrdinalIgnoreCase);
        }
    }
}
