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

        private static readonly List<string> ChapterTitleClasses = new List<string>
        {
            "chapter-title"
        };

        private static readonly List<string> ChapterClasses = new List<string>
        {
            "chapter-body"
        };

        private static readonly List<string> ChapterContentClasses = new List<string>
        {
            "translated"
        };

        public LNMTLSource() : base("LNMTL")
        {
        }

        public override async Task<WebNovelChapter> GetChapterAsync(ChapterLink link, ChapterRetrievalOptions options = default(ChapterRetrievalOptions),
            CancellationToken token = default(CancellationToken))
        {
            string content = await GetWebPageAsync(link.Url, token);

            IHtmlDocument doc = await Parser.ParseAsync(content, token);

            IElement titleElement = doc.DocumentElement.FirstWhereHasClass(ChapterTitleClasses);
            IElement chapterElement = doc.DocumentElement.FirstWhereHasClass(ChapterClasses);

            var contentEl = doc.CreateElement("P");
            var chContentElements = chapterElement.WhereHasClass(ChapterContentClasses, element => element.LocalName == "sentence");
            contentEl.Append(chContentElements.Cast<INode>().ToArray());

            string nextChapter = doc.QuerySelector("ul.pager > li.next > a")?.GetAttribute("href");

            return new WebNovelChapter
            {
                ChapterName = titleElement?.TextContent,
                Content = new ContentCleanup().Execute(doc, contentEl),
                NextChapterUrl = nextChapter
            };
        }
    }
}
