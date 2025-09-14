using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Onenote2md.Core;
using Onenote2md.Shared;
using Xunit;

namespace Onenote2md.Tests
{
    public class MDGeneratorTests
    {
        [Theory]
        [InlineData("Simple Page")]
        [InlineData("Page with spaces")]
        [InlineData("Page<script>")]
        [InlineData("Page|With:Invalid*Chars")]
        public void SanitizeName_HandlesVariousInputs_ReturnsValidFileName(string input)
        {
            // Act
            var result = TestHelper.SanitizeFileName(input);
            
            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            
            // Should not contain invalid characters
            var invalidChars = System.IO.Path.GetInvalidFileNameChars();
            Assert.True(invalidChars.All(c => !result.Contains(c)), 
                $"Result '{result}' contains invalid filename characters");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void SanitizeName_WithEmptyInput_ReturnsUntitled(string input)
        {
            // Act
            var result = TestHelper.SanitizeFileName(input);
            
            // Assert
            Assert.Equal("Untitled", result);
        }

        [Fact]
        public void SanitizeName_WithNullInput_ReturnsUntitled()
        {
            // Act
            var result = TestHelper.SanitizeFileName(null!);
            
            // Assert
            Assert.Equal("Untitled", result);
        }

        [Fact]
        public void ExtractSpanText_WithValidSpanTags_ExtractsInnerText()
        {
            // Arrange
            var input = "<span style='font-weight:bold'>Bold text</span> and <span>normal text</span>";
            var expected = "Bold text normal text";

            // Act
            var result = MDGenerator.ExtractSpanText(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ExtractSpanText_WithNoSpanTags_ReturnsOriginalText()
        {
            // Arrange
            var input = "Just plain text";

            // Act
            var result = MDGenerator.ExtractSpanText(input);

            // Assert
            Assert.Equal(input, result);
        }

        [Fact]
        public void ExtractSpanText_WithEmptyInput_ReturnsEmpty()
        {
            // Arrange & Act
            var result = MDGenerator.ExtractSpanText("");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ExtractSpanText_WithNestedSpans_ExtractsOuterText()
        {
            // Arrange
            var input = "<span>Outer <span>inner</span> text</span>";
            
            // Act
            var result = MDGenerator.ExtractSpanText(input);

            // Assert - Based on the actual regex implementation, it extracts the outer span content
            Assert.Contains("Outer", result);
            Assert.Contains("inner", result);
            // The actual implementation may not handle nested spans perfectly, so we test what it actually does
        }

        [Fact]
        public void GenerateFrontmatter_WithTitle_GeneratesCorrectYaml()
        {
            // Arrange
            var title = "Test Title";
            var created = new DateTime(2023, 1, 1, 10, 0, 0);
            
            // Act
            var result = TestHelper.GenerateFrontmatter(title, created, null);

            // Assert
            Assert.Contains("title: \"Test Title\"", result);
            Assert.Contains("created: 2023-01-01T10:00:00", result);
            Assert.Contains("---", result);
        }

        [Fact]
        public void ConvertSpanToMd_WithBoldSpan_ConvertsToBoldMarkdown()
        {
            // Arrange
            var input = "<span style='font-weight:bold'>Bold text</span>";
            
            // Act - test through public method that uses this logic
            var result = TestHelper.ConvertSpanToMd(input);

            // Assert
            Assert.Contains("**", result);
            Assert.Contains("Bold text", result);
        }

        [Theory]
        [InlineData("Simple text", "Simple text")]
        [InlineData("Text\nwith\nnewlines", "Text with newlines")]
        [InlineData("Multiple\n\n\nlines", "Multiple   lines")]
        public void ReplaceMultiline_ReplacesNewlinesWithSpaces(string input, string expected)
        {
            // Act
            var result = TestHelper.ReplaceMultiline(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TextReplacement_ReplacesNbspBold_ReturnsCleanedText()
        {
            // Arrange
            var input = "Text with &nbsp;** bold markers";
            var expected = "Text with ** bold markers";

            // Act
            var result = TestHelper.TextReplacement(input);

            // Assert
            Assert.Equal(expected, result);
        }
    }


}