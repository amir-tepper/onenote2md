using System;

namespace Onenote2md.Shared
{
    public class TagDef
    {
        public string Index { get; set; }
        public string Type { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }

        public MarkdownContent GetMD()
        {
            if (String.IsNullOrEmpty(Name))
                return MarkdownContent.Empty();
            else
            {
                switch (Name)
                {
                    case "To Do":
                    case "Teendő":
                        return MarkdownContent.SingleContent("[ ] ");

                    case "Important":
                    case "Fontos":
                        return MarkdownContent.SingleContent(":star: ");

                    case "Question":
                    case "Kérdés":
                        return MarkdownContent.SingleContent(":question: ");

                    case "Critical":
                    case "Kritikus":
                        return MarkdownContent.SingleContent(":exclamation: ");

                    case "Idea":
                    case "Ötlet":
                        return MarkdownContent.SingleContent(":bulb: ");


                    default:
                        return MarkdownContent.SingleContent(":red_circle: ");
                }
            }
        }
    }
}
