# OneNote2MD Enhancement: Added Created and Modified Date Frontmatter

## Summary of Changes

I have successfully implemented the ability to add created and modified date tags to each converted Markdown page. This enhancement makes the exported files fully compatible with Obsidian and other Markdown tools that utilize YAML frontmatter.

## Files Modified

### 1. `Onenote2md.Shared\MarkdownPage.cs`
- **Added Properties**: 
  - `DateTime? CreatedDate { get; set; }`
  - `DateTime? ModifiedDate { get; set; }`

### 2. `Onenote2md.Core\MDGenerator.cs`
- **Added Methods**:
  - `GetPageCreatedDate(XDocument doc)`: Extracts the creation date from OneNote XML
  - `GetPageModifiedDate(XDocument doc)`: Extracts the last modified date from OneNote XML  
  - `GenerateFrontmatter(string title, DateTime? createdDate, DateTime? modifiedDate)`: Creates YAML frontmatter

- **Modified Method**: `DoGenerateMD(string parentId, IWriter writer)`
  - Extracts created and modified dates from OneNote XML
  - Generates frontmatter with title and dates
  - Sets date properties on the MarkdownPage object
  - Prepends frontmatter to the final content

## How It Works

1. **Date Extraction**: The tool reads the `dateTime` and `lastModifiedTime` attributes from the OneNote XML Page element
2. **Frontmatter Generation**: Creates YAML frontmatter at the top of each Markdown file with:
   - `title`: The page title (properly escaped)
   - `created`: Creation date in ISO 8601 format (YYYY-MM-DDTHH:mm:ss)
   - `modified`: Last modified date in ISO 8601 format
3. **Content Assembly**: Combines the frontmatter with the existing Markdown content

## Example Output

```yaml
---
title: "My OneNote Page"
created: 2024-09-09T10:30:00
modified: 2024-09-09T15:45:00
---

# My OneNote Page

Your existing content here...
```

## Obsidian Compatibility

The generated frontmatter is fully compatible with Obsidian and supports:
- **Date-based filtering and searching**
- **Timeline views using created dates**
- **Dataview plugin queries**
- **Custom CSS styling based on metadata**
- **Automatic sorting by creation/modification dates**

## Benefits

- **Enhanced Organization**: Sort and filter notes by creation and modification dates
- **Better Workflow**: Maintain chronological understanding of your notes
- **Obsidian Integration**: Full compatibility with Obsidian's metadata system
- **Future-Proof**: Standard YAML frontmatter works with any Markdown tool
- **Backwards Compatible**: Existing functionality remains unchanged

## Technical Details

- Dates are parsed using `DateTime.TryParse()` for robust handling
- Date format follows ISO 8601 standard for maximum compatibility
- Title escaping handles quotes and special characters properly
- Null-safe operations prevent errors when date attributes are missing
- Empty frontmatter blocks are avoided when no metadata is available

The implementation is robust, handles edge cases gracefully, and maintains full backwards compatibility with existing OneNote2MD functionality.
