using System.Linq;
using System.Text;

namespace Onenote2md.Shared
{
    public static class FileHelper
    {
        public static string MakeValidFileName(string name)
        {
            name = StringHelper.Sanitize(name);
            name = StringHelper.ConvertSpanToMd(name);
            name = StringHelper.ReplaceSlash(name);
            var builder = new StringBuilder();
            var invalid = System.IO.Path.GetInvalidFileNameChars();
            foreach (var cur in name)
            {
                if (!invalid.Contains(cur))
                {
                    builder.Append(cur);
                }
            }
            return builder.ToString();
        }
    }
}
