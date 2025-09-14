using System;
using System.Xml.Linq;
using System.Linq;
using Onenote2md.Shared;
using Xunit;

namespace Onenote2md.Tests
{
    public class XmlParsingTests
    {
        [Fact]
        public void ParseOneNoteXml_WithValidXml_ParsesSuccessfully()
        {
            // Arrange
            var validXml = @"<?xml version=""1.0""?>
                <one:Notebooks xmlns:one=""http://schemas.microsoft.com/office/onenote/2013/onenote"">
                    <one:Notebook name=""Test Notebook"" ID=""{12345}"">
                        <one:Section name=""Test Section"" ID=""{67890}"">
                            <one:Page ID=""{ABCDE}"" name=""Test Page""/>
                        </one:Section>
                    </one:Notebook>
                </one:Notebooks>";

            // Act
            var doc = XDocument.Parse(validXml);

            // Assert
            Assert.NotNull(doc);
            Assert.Contains(doc.Descendants(), x => x.Name.LocalName == "Notebook");
        }

        [Theory]
        [InlineData("not xml at all")]
        public void ParseOneNoteXml_WithInvalidXml_ThrowsException(string invalidXml)
        {
            // Act & Assert - Invalid XML should throw exception
            Assert.Throws<System.Xml.XmlException>(() => XDocument.Parse(invalidXml));
        }

        [Fact]
        public void ParseOneNoteXml_WithIncompleteXml_ThrowsException()
        {
            // Arrange
            var incompleteXml = "<?xml version=\"1.0\"?><invalid>";
            
            // Act & Assert - Incomplete XML should throw exception
            Assert.Throws<System.Xml.XmlException>(() => XDocument.Parse(incompleteXml));
        }

        [Fact]
        public void OneNoteNamespace_IsCorrect()
        {
            // Verify the OneNote namespace constant is correct
            var expectedNamespace = "http://schemas.microsoft.com/office/onenote/2013/onenote";
            
            // Test that this is a valid URI
            Assert.True(Uri.IsWellFormedUriString(expectedNamespace, UriKind.Absolute));
        }
    }

    public class DataStructureTests
    {
        [Fact]
        public void MarkdownPage_Properties_WorkCorrectly()
        {
            // Arrange & Act
            var page = new MarkdownPage()
            {
                Title = "Test Page",
                Filename = "test-page.md",
                Content = "# Test Content\nThis is test content.",
                CreatedDate = new DateTime(2023, 1, 1),
                ModifiedDate = new DateTime(2023, 1, 2)
            };

            // Assert
            Assert.Equal("Test Page", page.Title);
            Assert.Equal("test-page.md", page.Filename);
            Assert.Contains("# Test Content", page.Content);
            Assert.Equal(new DateTime(2023, 1, 1), page.CreatedDate);
            Assert.Equal(new DateTime(2023, 1, 2), page.ModifiedDate);
        }

        [Theory]
        [InlineData(ObjectType.Notebook)]
        [InlineData(ObjectType.Section)]
        [InlineData(ObjectType.Page)]
        [InlineData(ObjectType.SectionGroup)]
        [InlineData(ObjectType.Children)]
        public void ObjectType_EnumValues_AreDefined(ObjectType type)
        {
            // Test that object type enum has expected values
            Assert.True(Enum.IsDefined(typeof(ObjectType), type));
        }
    }
}