using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Onenote2md.Shared
{
    public static class StringHelper
    {
        static Dictionary<string, string> spanReplacements = new Dictionary<string, string>()
        {
            { "<span style='font-weight:bold'>", " **" },
            { "<span style='font-weight:bold;text-decoration:underline'>", " **" },
            { "<span lang=hu>", "" },
            { "<span lang=en-US>", "" },
        };
        
        public static string Sanitize(String str)
        {
            str = str.Replace("^J", ",");
            str = str.Replace("\n", " ");
            str = str.Replace("<span lang=hu>", "");
            str = str.Replace("</span>", "");
            return str;
        }

        public static string ReplaceSlash(String str)
        {
            return str.Replace('/', '\uFF0F');
        }
        
        public static string TextReplacement(string source)
        {
            return source.Replace("&nbsp;**", "**");
        }

        public static string ReplaceMultiline(string source)
        {
            return source.Replace("\n", " ");
        }
        
        public static string ConvertSpanToMd(string source)
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
    }
}