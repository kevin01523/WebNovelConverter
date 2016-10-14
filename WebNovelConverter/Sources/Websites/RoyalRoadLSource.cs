using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using WebNovelConverter.Sources.Models;
using WebNovelConverter.Sources.Helpers;

namespace WebNovelConverter.Sources.Websites
{
    public class RoyalRoadLSource : WebNovelSource
    {
        public override string BaseUrl => "http://royalroadl.com/";

        public override List<Mode> AvailableModes => new List<Mode> { Mode.TableOfContents };

        private static readonly Regex RemoveFontStyleRegex = new Regex("(font|font-(family|size))\\s*:([^;]*)[;]?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public RoyalRoadLSource() : base("RoyalRoadL")
        {
        }

        public override async Task<IEnumerable<ChapterLink>> GetChapterLinksAsync(string baseUrl, CancellationToken token = default(CancellationToken))
        {
            string baseContent = await GetWebPageAsync(baseUrl, token);

            IHtmlDocument doc = await Parser.ParseAsync(baseContent, token);

            return CollectChapterLinks(baseUrl, doc.QuerySelectorAll("#chapters tbody tr[data-url] a"));
        }

        public override async Task<WebNovelChapter> GetChapterAsync(ChapterLink link,
            ChapterRetrievalOptions options = default(ChapterRetrievalOptions),
            CancellationToken token = default(CancellationToken))
        {
            string pageContent = await GetWebPageAsync(link.Url, token);

            IHtmlDocument doc = await Parser.ParseAsync(pageContent, token);

            IElement postBodyEl = doc.QuerySelector(".chapter-content");

            if (postBodyEl == null)
                return null;

            RemoveEmptyNodes(postBodyEl);
            RemoveAnnouncements(postBodyEl);
            RemoveNavigation(postBodyEl);
            RemoveAdvertisements(postBodyEl);
            ExpandSpoilers(postBodyEl);
            RemoveFontStyle(postBodyEl);

            var content = new ContentCleanup(BaseUrl).Execute(doc, postBodyEl);

            return new WebNovelChapter
            {
                Url = link.Url,
                Content = content
            };
        }

        public override async Task<WebNovelInfo> GetNovelInfoAsync(string baseUrl, CancellationToken token = default(CancellationToken))
        {
            string baseContent = await GetWebPageAsync(baseUrl, token);

            IHtmlDocument doc = await Parser.ParseAsync(baseContent, token);

            var coverUrl = doc.QuerySelector(".fiction-page .fic-header img")?.GetAttribute("src");
            var title = doc.QuerySelector(".fiction-page .fic-title h2")?.TextContent?.Trim();
            var description = doc.QuerySelector(".fiction-page .fiction-info .description")?.TextContent?.Trim();

            return new WebNovelInfo
            {
                CoverUrl = !string.IsNullOrWhiteSpace(coverUrl) ? coverUrl.Trim() : null,
                Title = !string.IsNullOrWhiteSpace(title) ? title.Trim() : null,
                Description = description
            };
        }

        protected virtual void RemoveEmptyNodes(IElement rootElement)
        {
            foreach (var node in rootElement.ChildNodes.ToList())
            {
                if (!node.HasChildNodes && string.IsNullOrWhiteSpace(node.TextContent) && node.NodeName.ToLower() != "br")
                {
                    rootElement.RemoveChild(node);
                }
            }
        }

        protected virtual void RemoveAnnouncements(IElement rootElement)
        {
            if (rootElement.FirstChild != null && rootElement.FirstChild.NodeName.ToLower() == "div")
                rootElement.RemoveChild(rootElement.FirstChild);
        }

        protected virtual void RemoveNavigation(IElement rootElement)
        {
            // Last 1-2 tables might be navigation

            foreach (var table in rootElement.QuerySelectorAll("table").Reverse().Take(2))
            {
                if (table.QuerySelectorAll("a").Any(x => x.TextContent.Contains("Chapter")))
                {
                    table.Remove();
                }
            }
        }

        protected virtual void RemoveAdvertisements(IElement rootElement)
        {
            // Donations
            foreach (var el in rootElement.QuerySelectorAll("div.thead"))
            {
                if (el.TextContent.Contains("Donation for the Author"))
                    el.Remove();
            }

            // Advertisements
            foreach (var el in rootElement.QuerySelectorAll("div.smalltext"))
            {
                el.Remove();
            }
        }

        /// <summary>
        /// Expands spoilers in HTML for easy reading.
        /// Expects:
        ///     <div class="spoiler_header">Spoilerxxx</div>
        ///     <div class="spoiler_body" style="display: none;">xxxx</div>
        /// </summary>
        /// <param name="rootElement"></param>
        protected void ExpandSpoilers(IElement rootElement)
        {
            foreach (IElement el in rootElement.QuerySelectorAll(".spoiler_body"))
            {
                el.SetAttribute("style", string.Empty);
                el.SetAttribute("class", string.Empty);

            }

            foreach (IElement el in rootElement.QuerySelectorAll(".spoiler_header"))
            {
                el.Remove();
            }
        }

        private void RemoveFontStyle(IElement rootElement)
        {
            foreach (var element in rootElement.Children.ToList())
            {
                RemoveFontStyle(element);

                if (element.HasAttribute("style"))
                {
                    var style = element.GetAttribute("style");
                    style = RemoveFontStyleRegex.Replace(style, string.Empty);
                    element.SetAttribute("style", style);
                }
            }
        }
    }
}
