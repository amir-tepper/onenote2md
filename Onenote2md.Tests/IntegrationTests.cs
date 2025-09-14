using System;
using System.IO;
using System.Linq;
using System.Text;
using Onenote2md.Shared;
using Xunit;

namespace Onenote2md.Tests
{
    public class IntegrationTests 
    {
        [Fact]
        public void TestWriter_WithSimplePage_StoresContent()
        {
            // Arrange
            var writer = new TestWriter();
            var testPage = new MarkdownPage()
            {
                Title = "Test Page", 
                Filename = "test-page.md",
                Content = @"# Test Page

This is a test page with some content.

## Section 1
Content for section 1."
            };

            // Act
            writer.WriteFile(testPage.Filename, testPage.Content);

            // Assert
            Assert.Single(writer.WrittenFiles);
            Assert.Equal("test-page.md", writer.WrittenFiles.First().Key);
            
            var content = writer.WrittenFiles.First().Value;
            Assert.Contains("# Test Page", content);
            Assert.Contains("## Section 1", content);
            Assert.Contains("This is a test page with some content.", content);
        }

        [Fact]
        public void TestWriter_WithMultiplePages_StoresAll()
        {
            // Arrange
            var writer = new TestWriter();

            // Act
            writer.WriteFile("parent-page.md", "# Parent Page\nThis is the parent page.");
            writer.WriteFile("parent-page/child-page.md", "# Child Page\nThis is a child page.");

            // Assert
            Assert.Equal(2, writer.WrittenFiles.Count);
            Assert.Contains("parent-page.md", writer.WrittenFiles.Keys);
            Assert.Contains("parent-page/child-page.md", writer.WrittenFiles.Keys);
        }

        [Fact]
        public void TestWriter_WithComplexFormatting_PreservesMarkdown()
        {
            // Arrange
            var writer = new TestWriter();
            var complexContent = @"---
title: ""Complex Page""
created: 2023-01-01T10:00:00
---

# Complex Page

This page has **bold text** and *italic text*.

## Table Example

| Column 1 | Column 2 | Column 3 |
|----------|----------|----------|
| Data 1   | Data 2   | Data 3   |
| More 1   | More 2   | More 3   |

## List Example

- Item 1
  - Sub item 1
  - Sub item 2
- Item 2

## Task List

- [ ] Todo item
- [x] Completed item";

            // Act
            writer.WriteFile("complex-page.md", complexContent);

            // Assert
            var result = writer.WrittenFiles["complex-page.md"];
            
            // Check frontmatter
            Assert.Contains("title: \"Complex Page\"", result);
            Assert.Contains("created: 2023-01-01T10:00:00", result);
            
            // Check formatting
            Assert.Contains("**bold text**", result);
            Assert.Contains("*italic text*", result);
            
            // Check table
            Assert.Contains("| Column 1 | Column 2 | Column 3 |", result);
            Assert.Contains("|----------|----------|----------|", result);
            
            // Check lists
            Assert.Contains("- Item 1", result);
            Assert.Contains("  - Sub item 1", result);
            
            // Check task list
            Assert.Contains("- [ ] Todo item", result);
            Assert.Contains("- [x] Completed item", result);
        }

        [Fact]
        public void TestWriter_WithInvalidInput_HandlesGracefully()
        {
            // Arrange
            var writer = new TestWriter();
            
            // Test with empty filename
            Assert.Throws<ArgumentException>(() => writer.WriteFile("", "content"));
            
            // Test with null content - should handle or throw appropriate exception
            // Test with null content - should throw exception
            Assert.Throws<ArgumentNullException>(() => writer.WriteFile("file.md", null!));
        }

        [Fact]
        public void TestWriter_WithUnicodeContent_PreservesCharacters()
        {
            // Arrange
            var writer = new TestWriter();
            var unicodeContent = @"# Unicode Test Page

## Emoji Test
Here are some emoji: 🎉 🚀 ⭐ 💡

## International Characters
English: Hello World
Spanish: Hola Mundo
Russian: Привет мир
Chinese: 你好世界
Japanese: こんにちは世界

## Special Characters
Math: α β γ δ ∑ ∫ ∞ ≈ ≠ ≤ ≥
Symbols: © ® ™ § ¶ † ‡ • ‰";

            // Act
            writer.WriteFile("unicode-page.md", unicodeContent);

            // Assert
            var result = writer.WrittenFiles["unicode-page.md"];
            
            // Check that Unicode characters are preserved
            Assert.Contains("🎉 🚀 ⭐ 💡", result);
            Assert.Contains("Привет мир", result);
            Assert.Contains("你好世界", result);
            Assert.Contains("こんにちは世界", result);
            Assert.Contains("α β γ δ", result);
            Assert.Contains("© ® ™", result);
        }

        [Fact]
        public void MarkdownPage_Properties_WorkCorrectly()
        {
            // Arrange & Act
            var page = new MarkdownPage()
            {
                Title = "Test Title",
                Content = "Test content",
                Filename = "test.md",
                CreatedDate = new DateTime(2023, 1, 1),
                ModifiedDate = new DateTime(2023, 1, 2)
            };

            // Assert
            Assert.Equal("Test Title", page.Title);
            Assert.Equal("Test content", page.Content);
            Assert.Equal("test.md", page.Filename);
            Assert.Equal(new DateTime(2023, 1, 1), page.CreatedDate);
            Assert.Equal(new DateTime(2023, 1, 2), page.ModifiedDate);
        }
    }

    /// <summary>
    /// Extended test writer that tracks metrics
    /// </summary>
    public class ExtendedTestWriter : TestWriter
    {
        public int WriteCount { get; private set; }
        public DateTime LastWriteTime { get; private set; }
        public long TotalBytesWritten { get; private set; }

        public override void WriteFile(string filename, string content)
        {
            WriteCount++;
            LastWriteTime = DateTime.Now;
            TotalBytesWritten += Encoding.UTF8.GetByteCount(content ?? "");

            base.WriteFile(filename, content ?? "");
        }

        public void Reset()
        {
            WriteCount = 0;
            LastWriteTime = default;
            TotalBytesWritten = 0;
            WrittenFiles.Clear();
        }
    }
}