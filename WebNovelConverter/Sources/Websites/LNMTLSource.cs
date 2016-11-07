using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using WebNovelConverter.Extensions;
using WebNovelConverter.Sources.Models;
using WebNovelConverter.Sources.Helpers;

namespace WebNovelConverter.Sources.Websites
{
    public class LNMTLSource : WebNovelSource
    {
        public override string BaseUrl => "http://lnmtl.com";

        public override List<Mode> AvailableModes => new List<Mode> { Mode.NextChapterLink };

        public LNMTLSource() : base("LNMTL")
        {
        }

        public override async Task<WebNovelChapter> GetChapterAsync(ChapterLink link, ChapterRetrievalOptions options = default(ChapterRetrievalOptions),
            CancellationToken token = default(CancellationToken))
        {
            string content = await GetWebPageAsync(link.Url, token);

            IHtmlDocument doc = await Parser.ParseAsync(content, token);

            IElement titleElement = doc.DocumentElement.QuerySelector(".chapter-title");
            IElement chapterElement = doc.DocumentElement.QuerySelector(".chapter-body");

            // Append paragraphs after each "sentence.translated" element.
            chapterElement
                .QuerySelectorAll("sentence.translated")
                .ToList()
                .ForEach((obj) => obj.AppendChild(doc.CreateElement("P")));
            var contentEl = doc.CreateElement("P");
            contentEl.InnerHtml = string.Join("", chapterElement
                .QuerySelectorAll("sentence.translated")
                .Select(x => x.InnerHtml));
            RemoveSpecialTags(doc, contentEl);

            string nextChapter = doc.QuerySelector("ul.pager > li.next > a")?.GetAttribute("href");

            return new WebNovelChapter
            {
                ChapterName = titleElement?.GetInnerText(),
                Content = new ContentCleanup(BaseUrl).Execute(doc, contentEl),
                NextChapterUrl = nextChapter
            };
        }

        private void RemoveSpecialTags(IDocument doc, IElement element)
        {
            element.ForAllElements(child =>
            {
                if(child.NodeName == "DQ")
                {
                    var newEl = doc.CreateElement("DQ");
                    newEl.TextContent = child.GetInnerText();
                    child.ReplaceWith(newEl);
                }
                else if(child.NodeName == "W" || child.NodeName == "T")
                {
                    child.ReplaceWith(doc.CreateTextNode(" " + child.GetInnerText()));
                }
            });
        }
    }
}
