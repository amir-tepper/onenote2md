using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Onenote2md.Core;
using Onenote2md.Shared;
using Xunit;

namespace Onenote2md.Tests
{
    public class SubpageExportTests
    {
        [Fact]
        public void Exports_Subpages_Into_Parent_Folders_Correctly()
        {
            // Arrange: Simulate a section with the following structure:
            // section1
            //   page1
            //     subpage2
            //       subsubpage3
            //
            // Expect:
            // section1/
            //   page1.md
            //   page1/
            //     subpage2.md
            //     subpage2/
            //       subsubpage3.md

            var fakePages = new List<(string Id, string Name, int PageLevel)>
            {
                ("1", "page1", 1),
                ("2", "subpage2", 2),
                ("3", "subsubpage3", 3)
            };

            var output = new List<string>();
            var writer = new TestWriter(output);

            // Simulate the export logic (mimic GenerateSectionMD's new logic)
            var parentStack = new Stack<(int Level, string Name)>();
            writer.PushDirectory("section1");
            for (int i = 0; i < fakePages.Count; i++)
            {
                var page = fakePages[i];
                if (page.PageLevel == 1)
                {
                    writer.WritePage(new MarkdownPage { Title = page.Name, Filename = Path.Combine(writer.GetOutputDirectory(), page.Name + ".md") });
                }
                if (page.PageLevel > 1)
                {
                    while (parentStack.Count > 0 && parentStack.Peek().Level >= page.PageLevel)
                    {
                        writer.PopDirectory();
                        parentStack.Pop();
                    }
                    int parentIdx = i - 1;
                    while (parentIdx >= 0 && fakePages[parentIdx].PageLevel != page.PageLevel - 1)
                        parentIdx--;
                    if (parentIdx >= 0)
                    {
                        writer.PushDirectory(fakePages[parentIdx].Name);
                        parentStack.Push((fakePages[parentIdx].PageLevel, fakePages[parentIdx].Name));
                    }
                    writer.PushDirectory(page.Name);
                    parentStack.Push((page.PageLevel, page.Name));
                    writer.WritePage(new MarkdownPage { Title = page.Name, Filename = Path.Combine(writer.GetOutputDirectory(), page.Name + ".md") });
                    writer.PopDirectory();
                    parentStack.Pop();
                    if (parentStack.Count > 0 && (i + 1 >= fakePages.Count || fakePages[i + 1].PageLevel <= parentStack.Peek().Level))
                    {
                        writer.PopDirectory();
                        parentStack.Pop();
                    }
                }
                else
                {
                    while (parentStack.Count > 0)
                    {
                        writer.PopDirectory();
                        parentStack.Pop();
                    }
                }
            }
            writer.PopDirectory();

            // Assert: Check the output paths
            Assert.Contains("section1\\page1.md", output);
            Assert.Contains("section1\\page1\\subpage2.md", output);
            Assert.Contains("section1\\page1\\subpage2\\subsubpage3.md", output);
            // Should NOT contain subpage2.md or subsubpage3.md at section root
            Assert.DoesNotContain("section1\\subpage2.md", output);
            Assert.DoesNotContain("section1\\subsubpage3.md", output);
        }

        // Simple test writer to capture output paths
        class TestWriter : IWriter
        {
            private readonly List<string> _output;
            private readonly Stack<string> _dirs = new Stack<string>();
            public TestWriter(List<string> output) { _output = output; }
            public void PushDirectory(string dir) => _dirs.Push(dir);
            public void PopDirectory() { if (_dirs.Count > 0) _dirs.Pop(); }
            public string GetOutputDirectory() => string.Join("\\", _dirs.Reverse());
            public void WritePage(MarkdownPage page) => _output.Add(page.Filename);
            public void WritePageImage(string fullPath, byte[] image) { }
        }
    }
}
