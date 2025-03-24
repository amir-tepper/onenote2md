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
                switch ((TagDefType)int.Parse(Type))
                {
                    case TagDefType.ToDo:
                        return MarkdownContent.SingleContent("[ ] ");

                    case TagDefType.Important:
                        return MarkdownContent.SingleContent(":star: ");

                    case TagDefType.Question:
                        return MarkdownContent.SingleContent(":question: ");

                    case TagDefType.Critical:
                        return MarkdownContent.SingleContent(":exclamation: ");

                    case TagDefType.Idea:
                        return MarkdownContent.SingleContent(":bulb: ");


                    default:
                        return MarkdownContent.SingleContent(":red_circle: ");
                }
            }
        }
    }
}
