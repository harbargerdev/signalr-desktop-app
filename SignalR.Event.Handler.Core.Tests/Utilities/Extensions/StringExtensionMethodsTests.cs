using FluentAssertions;
using NUnit.Framework;
using SignalR.Event.Handler.Core.Utilities.Extensions;

namespace SignalR.Event.Handler.Core.Tests.Utilities.Extensions
{
    [TestFixture]
    public class StringExtensionMethodsTests
    {
        [Test]
        public void IsNullOrEmptyTrimmed_NullString_ReturnsTrue()
        {
            // Arrange
            string? str = null;

            // Act
            var result = str.IsNullOrEmptyTrimmed();

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsNullOrEmptyTrimmed_EmptyString_ReturnsTrue()
        {
            // Arrange
            var str = string.Empty;

            // Act
            var result = str.IsNullOrEmptyTrimmed();

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsNullOrEmptyTrimmed_WhitespaceOnly_ReturnsTrue()
        {
            // Arrange
            var str = "   ";

            // Act
            var result = str.IsNullOrEmptyTrimmed();

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsNullOrEmptyTrimmed_TabsAndNewlines_ReturnsTrue()
        {
            // Arrange
            var str = "\t\n\r";

            // Act
            var result = str.IsNullOrEmptyTrimmed();

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void IsNullOrEmptyTrimmed_NonEmptyString_ReturnsFalse()
        {
            // Arrange
            var str = "Hello";

            // Act
            var result = str.IsNullOrEmptyTrimmed();

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void IsNullOrEmptyTrimmed_StringWithLeadingTrailingSpaces_ReturnsFalse()
        {
            // Arrange
            var str = "  Hello  ";

            // Act
            var result = str.IsNullOrEmptyTrimmed();

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void CompareIgnoreCase_BothNull_ReturnsTrue()
        {
            // Arrange
            string? str1 = null;
            string? str2 = null;

            // Act
            var result = str1.CompareIgnoreCase(str2);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void CompareIgnoreCase_OneNull_ReturnsFalse()
        {
            // Arrange
            string? str1 = "Hello";
            string? str2 = null;

            // Act
            var result = str1.CompareIgnoreCase(str2);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void CompareIgnoreCase_SameCase_ReturnsTrue()
        {
            // Arrange
            var str1 = "Hello";
            var str2 = "Hello";

            // Act
            var result = str1.CompareIgnoreCase(str2);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void CompareIgnoreCase_DifferentCase_ReturnsTrue()
        {
            // Arrange
            var str1 = "Hello";
            var str2 = "HELLO";

            // Act
            var result = str1.CompareIgnoreCase(str2);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void CompareIgnoreCase_MixedCase_ReturnsTrue()
        {
            // Arrange
            var str1 = "HeLLo WoRLd";
            var str2 = "hello WORLD";

            // Act
            var result = str1.CompareIgnoreCase(str2);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void CompareIgnoreCase_DifferentContent_ReturnsFalse()
        {
            // Arrange
            var str1 = "Hello";
            var str2 = "Goodbye";

            // Act
            var result = str1.CompareIgnoreCase(str2);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void CompareIgnoreCase_EmptyStrings_ReturnsTrue()
        {
            // Arrange
            var str1 = string.Empty;
            var str2 = string.Empty;

            // Act
            var result = str1.CompareIgnoreCase(str2);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void CompareIgnoreCase_WhitespacePreserved_ReturnsFalse()
        {
            // Arrange
            var str1 = "Hello World";
            var str2 = "HelloWorld";

            // Act
            var result = str1.CompareIgnoreCase(str2);

            // Assert
            result.Should().BeFalse();
        }
    }
}
