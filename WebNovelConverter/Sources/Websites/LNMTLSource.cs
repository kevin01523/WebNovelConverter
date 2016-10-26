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

            var contentEl = doc.CreateElement("P");
            contentEl.InnerHtml = string.Join("", chapterElement
                .QuerySelectorAll("sentence.translated")
                .Select(x => x.InnerHtml));
            CreateParagraphs(doc, contentEl);
            RemoveSpecialTags(doc, contentEl);

            string nextChapter = doc.QuerySelector("ul.pager > li.next > a")?.GetAttribute("href");

            return new WebNovelChapter
            {
                ChapterName = titleElement?.GetInnerText(),
                Content = new ContentCleanup(BaseUrl).Execute(doc, contentEl),
                NextChapterUrl = nextChapter
            };
        }

        private void CreateParagraphs(IDocument doc, IElement element)
        {
            foreach (var child in element.ChildNodes.ToList())
            {
                if (child.NodeType == NodeType.Element && child.NodeName == "DQ")
                {
                    ContentCleanup.ReplaceElementWithParagraph(doc, child);
                }
            }
        }

        private void RemoveSpecialTags(IDocument doc, IElement element)
        {
            element.ForAllElements(child =>
            {
                if(child.NodeName == "W" || child.NodeName == "T")
                {
                    child.ReplaceWith(doc.CreateTextNode(" " + child.GetInnerText()));
                }
            });
        }
    }
}
