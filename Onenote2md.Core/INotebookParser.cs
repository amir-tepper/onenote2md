using System.Collections.Generic;
using System.Xml.Linq;
using Onenote2md.Shared;

namespace Onenote2md.Core
{
    public interface INotebookParser
    {
        /// <summary>
        /// Gets XML document from OneNote hierarchy
        /// </summary>
        XDocument GetXDocument(string parentId, object scope);
        
        /// <summary>
        /// Gets object ID by name within a specific scope
        /// </summary>
        string GetObjectId(object scope, string objectName);
        
        /// <summary>
        /// Gets child object map (ID to name mapping) for a specific object type
        /// </summary>
        IDictionary<string, string> GetChildObjectMap(string parentId, object scope, ObjectType requestedObject);
        
        /// <summary>
        /// Gets the OneNote Application instance
        /// </summary>
        object GetOneNoteApp();
        
        /// <summary>
        /// Closes a OneNote notebook
        /// </summary>
        void Close(string notebookId);

        
        string GetBinaryPageContent(string parentId, string id);
    }
}