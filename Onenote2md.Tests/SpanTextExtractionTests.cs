using System;
using Xunit;
using Onenote2md.Core;

namespace Onenote2md.Tests
{
    public class SpanTextExtractionTests
    {
        [Fact]
        public void ExtractSpanText_Concatenates_Inner_Texts()
        {
            // Arrange
            string input = "<span lang=en-US>Page 1 </span><span style='direction:rtl;unicode-bidi:embed' lang=he>\u05e2\u05de\u05d5\u05d3</span>";
            string expected = "Page 1 \u05e2\u05de\u05d5\u05d3";

            // Act
            string result = MDGenerator.ExtractSpanText(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ExtractSpanText_Ignores_Empty_And_Whitespace_Spans()
        {
            string input = "<span lang=en-US> </span><span style='direction:rtl;unicode-bidi:embed' lang=he>\u05e2\u05de\u05d5\u05d3</span>";
            string expected = "\u05e2\u05de\u05d5\u05d3";
            string result = MDGenerator.ExtractSpanText(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ExtractSpanText_Returns_Empty_For_No_Spans()
        {
            string input = "No spans here";
            string result = MDGenerator.ExtractSpanText(input);
            Assert.Equal(input, result); // output would be the same if there are not spans
        }
    }
}
