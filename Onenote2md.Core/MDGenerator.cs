using Onenote2md.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Microsoft.Office.Interop.OneNote;

namespace Onenote2md.Core
{
    public class MDGenerator : IGenerator
    {
        #region Fields
        private INotebookParser parser;
        private XNamespace ns;

        static Dictionary<string, string> spanReplacements = new Dictionary<string, string>()
        {
            { "<span style='font-weight:bold'>", " **" },
            { "<span style='font-weight:bold;text-decoration:underline'>", " **" }
        };
        #endregion

        #region Constructors
        public MDGenerator(INotebookParser parser)
        {
            this.parser = parser;
            var doc = parser.GetXDocument(
                null, HierarchyScope.hsNotebooks);
            ns = doc.Root.Name.Namespace;
        }
        #endregion

        #region Helpers
        protected string NormalizeName(string source)
        {
            string ns = "{http://schemas.microsoft.com/office/onenote/2013/onenote}";
            if (String.IsNullOrWhiteSpace(source))
                return source;

            if (source.StartsWith(ns))
                source = source.Replace(ns, "");

            return source;
        }

        protected string TextReplacement(string source)
        {
            return source.Replace("&nbsp;**", "**");
        }

        protected string ReplaceMultiline(string source)
        {
            return source.Replace("\n", " ");
        }

        protected string ConvertSpanToMd(string source)
        {
            foreach (var item in spanReplacements)
            {
                if (source.Contains(item.Key))
                {
                    source = source.Replace(item.Key, item.Value);
                    source = source.Replace("** ", "**");
                    source = source.Replace("</span>&nbsp;", item.Value.Trim());
                    source = source.Replace("</span>", item.Value.Trim());
                    //source = source.Replace("&nbsp;>", " ");
                    break;
                }
            }

            return source;
        }

        protected string GetAttibuteValue(XElement element, string attributeName)
        {
            var v = element.Attributes().Where(q => q.Name == attributeName).FirstOrDefault();
            if (v != null)
                return v.Value;
            else
                return null;
        }

        protected string GetElementValue(XElement element)
        {
            return element.Value;
        }

        // Extracts and concatenates inner text from all <span>...</span> tags, ignoring attributes
        public static string ExtractSpanText(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var matches = System.Text.RegularExpressions.Regex.Matches(input, @"<span[^>]*>(.*?)</span>", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
            if (matches.Count == 0)
            {
                return input;
            }
            var result = string.Join(" ", matches.Cast<System.Text.RegularExpressions.Match>().Select(m => m.Groups[1].Value.Trim()).Where(s => !string.IsNullOrEmpty(s)));
            return result.Trim();
        }
        #endregion

        #region IGenerator
        public MarkdownPage PreviewMD(string parentId)
        {
            MDWriter tempWriter = new MDWriter(@"c:\temp\onenote2md", true);
            return DoGenerateMD(parentId, tempWriter);
        }

        public void GeneratePageMD(string parentId, IWriter writer)
        {
            var md = DoGenerateMD(parentId, writer);
            writer.WritePage(md);
        }

        public void GenerateSectionMD(string sectionName, IWriter writer)
        {
            var sectionId = parser.GetObjectId(
                Microsoft.Office.Interop.OneNote.HierarchyScope.hsSections, sectionName);

            GenerateSectionMD(sectionId, sectionName, writer);
        }

        public void GenerateSectionMD(string sectionId, string sectionName, IWriter writer)
        {
            if (!String.IsNullOrEmpty(sectionId))
            {
                // Get all Page elements in order, with their ID, name, and pageLevel
                var doc = parser.GetXDocument(sectionId, Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren);
                var ns = doc.Root.Name.Namespace;

                var pages = doc.Descendants(ns + "Page")
                    .Select(p => new {
                        Id = p.Attribute("ID")?.Value,
                        Name = SanitizeName(p.Attribute("name")?.Value),
                        PageLevel = int.TryParse(p.Attribute("pageLevel")?.Value, out int lvl) ? lvl : 1
                    })
                    .Where(p => !string.IsNullOrEmpty(p.Id) && !string.IsNullOrEmpty(p.Name))
                    .ToList();

                // Stack to track parent page names for folder structure
                var parentStack = new Stack<(int Level, string Name)>();

                writer.PushDirectory(sectionName);
                try
                {
                    for (int i = 0; i < pages.Count; i++)
                    {
                        var page = pages[i];
                        // Pop to the correct parent level
                        while (parentStack.Count > 0 && parentStack.Peek().Level >= page.PageLevel)
                        {
                            writer.PopDirectory();
                            parentStack.Pop();
                        }
                        // For subpages, push parent chain
                        if (page.PageLevel > 1)
                        {
                            // Find the parent page (the nearest previous page with level one less)
                            int parentIdx = i - 1;
                            while (parentIdx >= 0 && pages[parentIdx].PageLevel != page.PageLevel - 1)
                                parentIdx--;
                            if (parentIdx >= 0 && (parentStack.Count == 0 || parentStack.Peek().Name != pages[parentIdx].Name))
                            {
                                writer.PushDirectory(pages[parentIdx].Name);
                                parentStack.Push((pages[parentIdx].PageLevel, pages[parentIdx].Name));
                            }
                        }
                        // Write the page in the current directory
                        GeneratePageMD(page.Id, writer);
                    }
                }
                finally
                {
                    // Pop any remaining directories
                    while (parentStack.Count > 0)
                    {
                        writer.PopDirectory();
                        parentStack.Pop();
                    }
                    writer.PopDirectory();
                }
            }
        }

        // Remove leading/trailing spaces, illegal path characters, and tag-like patterns from page/section names
        private static string SanitizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Untitled";

            // Remove leading/trailing whitespace
            name = name.Trim();


            // Remove all attribute patterns (e.g., lang=he, style='...', etc.)
            // Handles: lang=he, style='...', style="...", style=word
            name = System.Text.RegularExpressions.Regex.Replace(name, @"\b\w+\s*=\s*'[^']*'", "");
            name = System.Text.RegularExpressions.Regex.Replace(name, @"\b\w+\s*=\s*""[^""]*""", "");
            name = System.Text.RegularExpressions.Regex.Replace(name, @"\b\w+\s*=\s*[^\s]+", "");

            // Remove all standalone tag names (e.g., span, spanspanstyle) at word boundaries
            name = System.Text.RegularExpressions.Regex.Replace(name, @"\b(span|spanspanstyle)\b", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Collapse multiple spaces
            name = System.Text.RegularExpressions.Regex.Replace(name, @"\s+", " ");

            // Remove any leftover angle brackets (if any)
            name = name.Replace("<", "").Replace(">", "");

            // Remove illegal path characters
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }

            // Final trim
            name = name.Trim();

            // If the result is empty, return Untitled
            if (string.IsNullOrWhiteSpace(name))
                return "Untitled";

            return name;
        }

        public void GenerateSectionGroupMD(string sectionGroupId, string sectionGroupName, IWriter writer)
        {
            if (!String.IsNullOrEmpty(sectionGroupId))
            {
                var subSectionGroups = parser.GetChildObjectMap(
                    sectionGroupId, Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren,
                    ObjectType.SectionGroup);

                try
                {
                    writer.PushDirectory(sectionGroupName);

                    foreach (var item in subSectionGroups)
                    {
                        GenerateSectionGroupMD(item.Key, item.Value, writer);
                    }

                    var subSection = parser.GetChildObjectMap(
                        sectionGroupId, Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren,
                        ObjectType.Section);

                    foreach (var section in subSection)
                    {
                        GenerateSectionMD(section.Key, section.Value, writer);
                    }
                }
                finally
                {
                    writer.PopDirectory();
                }
            }
        }


        public void GenerateNotebookMD(string notebookName, IWriter writer)
        {
            var notebookId = parser.GetObjectId(
                Microsoft.Office.Interop.OneNote.HierarchyScope.hsNotebooks, notebookName);

            if (!String.IsNullOrEmpty(notebookId))
            {
                var subSectionGroups = parser.GetChildObjectMap(
                    notebookId, Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren,
                    ObjectType.SectionGroup);

                foreach (var item in subSectionGroups)
                {
                    GenerateSectionGroupMD(item.Key, item.Value, writer);
                }

                var subSection = parser.GetChildObjectMap(
                    notebookId, Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren,
                    ObjectType.Section);

                foreach (var section in subSection)
                {
                    GenerateSectionMD(section.Key, section.Value, writer);
                }
            }
        }

        protected MarkdownPage DoGenerateMD(string parentId, IWriter writer)
        {
            MarkdownPage markdownPage = new MarkdownPage();

            var scope = Microsoft.Office.Interop.OneNote.HierarchyScope.hsChildren;
            var doc = parser.GetXDocument(parentId, scope);

            StringBuilder mdContent = new StringBuilder();

            // create context
            var quickStyles = GetQuickStyleDef(doc);
            var tags = GetTagDef(doc);
            var context = new MarkdownGeneratorContext(writer, parentId, quickStyles, tags);
            var pageTitle = GetPageTitle(doc);
            context.SetPageTitle(pageTitle);

            // Extract page dates
            var createdDate = GetPageCreatedDate(doc);
            var modifiedDate = GetPageModifiedDate(doc);

            var titleElement = GetTitleElement(doc);
            GenerateChildObjectMD(titleElement, context, 0, mdContent);


            var childenContent = DoGenerateMDRoots("OEChildren", doc, context);
            if (String.IsNullOrWhiteSpace(childenContent))
            {
                var directImageContent = DoGenerateMDRoots("Image", doc, context);
                if (!String.IsNullOrWhiteSpace(directImageContent))
                    mdContent.Append(directImageContent);
            }
            else
            {
                mdContent.Append(childenContent);
            }


            // Generate frontmatter
            var frontmatter = GenerateFrontmatter(context.GetPageTitle(), createdDate, modifiedDate);
            var finalContent = frontmatter + mdContent.ToString();

            markdownPage.Content = finalContent;
            markdownPage.Title = context.GetPageTitle();
            markdownPage.Filename = context.GetPageFullPath();
            markdownPage.CreatedDate = createdDate;
            markdownPage.ModifiedDate = modifiedDate;

            return markdownPage;
        }

        protected string DoGenerateMDRoots(string rootNodeName, XDocument doc, MarkdownGeneratorContext context)
        {
            var result = new StringBuilder();

            // Find all OEChildren nodes in the document
            var childrenNodes = doc.Descendants(ns + rootNodeName).ToList();
            foreach (var children in childrenNodes)
            {
                if (children != null && children.HasElements)
                {
                    var rootElements = children.Elements().ToList();
                    foreach (var rootElement in rootElements)
                    {
                        int level = 0;
                        GenerateChildObjectMD(rootElement, context, level, result);
                    }
                }
            }

            if (context.HasPairedContent())
            {
                result.Append(context.Get().Content);
                context.Reset();
            }

            return result.ToString();
        }
        #endregion

        #region Generation helpers
        protected Dictionary<string, QuickStyleDef> GetQuickStyleDef(XDocument doc)
        {
            var nodeName = "QuickStyleDef";

            var result = new Dictionary<string, QuickStyleDef>();
            var quickStyleDefs = doc.Descendants(ns + nodeName);
            if (quickStyleDefs != null)
            {
                foreach (var item in quickStyleDefs)
                {
                    QuickStyleDef def = new QuickStyleDef();
                    def.Index = GetAttibuteValue(item, "index");
                    def.Name = GetAttibuteValue(item, "name");
                    result.Add(def.Index, def);
                }

            }

            return result;
        }

        protected Dictionary<string, TagDef> GetTagDef(XDocument doc)
        {
            var nodeName = "TagDef";

            var result = new Dictionary<string, TagDef>();
            var tagDefs = doc.Descendants(ns + nodeName);
            if (tagDefs != null)
            {
                foreach (var item in tagDefs)
                {
                    var def = new TagDef();
                    def.Index = GetAttibuteValue(item, "index");
                    def.Name = GetAttibuteValue(item, "name");
                    def.Symbol = GetAttibuteValue(item, "symbol");
                    def.Type = GetAttibuteValue(item, "type");
                    result.Add(def.Index, def);
                }
            }

            return result;
        }

        protected string GetPageTitle(XDocument doc)
        {
            var nodeName = "Title";

            var element = doc.Descendants(ns + nodeName).FirstOrDefault();
            if (element != null)
            {
                var title = element.Descendants(ns + "OE").FirstOrDefault();
                if (title != null)
                    return ExtractSpanText(title.Value.ToString());
            }

            return "Untitled";
        }

        protected XElement GetTitleElement(XDocument doc)
        {
            var nodeName = "Title";

            var element = doc.Descendants(ns + nodeName).FirstOrDefault();

            return element;
        }

        protected DateTime? GetPageCreatedDate(XDocument doc)
        {
            var pageElement = doc.Descendants(ns + "Page").FirstOrDefault();
            if (pageElement != null)
            {
                var dateTimeAttr = GetAttibuteValue(pageElement, "dateTime");
                if (!string.IsNullOrEmpty(dateTimeAttr))
                {
                    if (DateTime.TryParse(dateTimeAttr, out DateTime result))
                        return result;
                }
            }
            return null;
        }

        protected DateTime? GetPageModifiedDate(XDocument doc)
        {
            var pageElement = doc.Descendants(ns + "Page").FirstOrDefault();
            if (pageElement != null)
            {
                var lastModifiedAttr = GetAttibuteValue(pageElement, "lastModifiedTime");
                if (!string.IsNullOrEmpty(lastModifiedAttr))
                {
                    if (DateTime.TryParse(lastModifiedAttr, out DateTime result))
                        return result;
                }
            }
            return null;
        }

        protected string GenerateFrontmatter(string title, DateTime? createdDate, DateTime? modifiedDate)
        {
            var frontmatter = new StringBuilder();
            frontmatter.AppendLine("---");
            
            if (!string.IsNullOrEmpty(title))
            {
                frontmatter.AppendLine($"title: \"{title.Replace("\"", "\\\"")}\"");
            }

            if (createdDate.HasValue)
            {
                frontmatter.AppendLine($"created: {createdDate.Value:yyyy-MM-ddTHH:mm:ss}");
            }

            if (modifiedDate.HasValue)
            {
                frontmatter.AppendLine($"modified: {modifiedDate.Value:yyyy-MM-ddTHH:mm:ss}");
            }

            frontmatter.AppendLine("---");
            frontmatter.AppendLine();
            
            return frontmatter.ToString();
        }

        protected void GenerateChildObjectMD(
            XElement node, MarkdownGeneratorContext context, long level, StringBuilder results)
        {
            if (node != null)
            {
                string name = NormalizeName(node.Name.ToString());
                bool stdTraversal = true;


                StringBuilder content = new StringBuilder();
                switch (name)
                {
                    case "OE":
                        {
                            if (context.HasPairedContent())
                            {
                                content.Append(context.Get().Content);
                                context.Reset();
                            }

                            if (context.TableInfo.IsOnTable())
                            {

                            }
                            else
                            {
                                var quickStyleIndex = GetAttibuteValue(node, "quickStyleIndex");
                                if (!String.IsNullOrEmpty(quickStyleIndex))
                                {
                                    var quickStyleDef = context.GetQuickStyleDef(quickStyleIndex);
                                    if (quickStyleDef != null)
                                    {
                                        var mdContent = quickStyleDef.GetMD();
                                        if (!mdContent.WillAppendLine())
                                            content.AppendLine();
                                        context.Set(mdContent);
                                        content.Append(mdContent.Content);
                                    }
                                }
                            }
                        }
                        break;

                    case "T":
                        {
                            string v = ReplaceMultiline(node.Value);

                            v = ConvertSpanToMd(v);
                            v = TextReplacement(v);
                            content.Append(v);
                        }
                        break;

                    case "Bullet":
                        {
                            content.Append("- ");
                        }
                        break;

                    case "Number":
                        {
                            content.Append("1. ");
                        }
                        break;

                    case "Tag":
                        {
                            var tagIndex = GetAttibuteValue(node, "index");
                            var tagDef = context.GetTagDef(tagIndex);

                            if (tagDef != null)
                            {
                                if (tagDef.Name.Equals("To Do", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var completed = GetAttibuteValue(node, "completed");
                                    if (completed == "true")
                                        content.Append("- [x] ");
                                    else
                                        content.Append("- [ ] ");
                                }
                                else
                                {
                                    var tagMdContent = tagDef.GetMD();
                                    content.Append("- ");
                                    content.Append(tagMdContent.Content);
                                }
                            }
                        }
                        break;



                    case "Table":
                        {
                            if (context.HasPairedContent())
                            {
                                content.Append(context.Get().Content);
                                context.Reset();
                            }

                            stdTraversal = false;
                            context.TableInfo.SetOnTable();
                            results.Append(content);

                            if (node.HasElements)
                            {
                                var subs = node.Elements().ToList();
                                foreach (var item in subs)
                                {
                                    GenerateChildObjectMD(item, context, ++level, results);
                                }
                            }

                            results.AppendLine();
                            context.TableInfo.Reset();
                        }
                        break;

                    case "Column":
                        {
                            context.TableInfo.AppendTableColumn();
                        }
                        break;

                    case "Row":
                        {
                            if (context.TableInfo.IsOnTable())
                            {
                                if (context.TableInfo.OnHeaderRow())
                                {
                                    content.AppendLine();
                                    var columns = context.TableInfo.GetTableColumnCount();
                                    for (int i = 0; i < columns; i++)
                                    {
                                        content.Append("| - ");
                                    }
                                    content.Append("|");
                                }

                                stdTraversal = false;

                                content.AppendLine();
                                context.TableInfo.AppendRow();

                                results.Append(content);
                                // ...existing code...
                            }
                        }
                        break;
                    // ...existing code for other cases...
                }
            }
        }
    }
    #endregion
}
