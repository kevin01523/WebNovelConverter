using AngleSharp.Dom;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebNovelConverter.Sources.Helpers
{
    public class ContentCleanup
    {
        private string _baseUrl;

        public ContentCleanup(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public string Execute(IDocument doc, IElement element)
        {
            RemoveUnneededElements(element);
            RemoveEmptyElements(element);
            RemoveEmptyTextNodes(element);
            RemoveMultipleBr(element);
            RemoveBeginEndBr(element);

            CreateParagraphs(doc, element);

            MakeURLsAbsolute(doc, element);

            return Regex.Replace(element.InnerHtml, "[ ]{2,}", " ").Trim();
        }

        private void MakeURLsAbsolute(IDocument doc, IElement element)
        {
            if (_baseUrl != null)
            {
                foreach (IElement el in element.QuerySelectorAll("img").ToList())
                {
                    var src = el.Attributes["src"]?.Value;
                    if (src != null && src.StartsWith("/"))
                    {
                        el.SetAttribute("src", UrlHelper.ToAbsoluteUrl(_baseUrl, src));
                    }
                }
            }
        }

        private void RemoveUnneededElements(IElement rootElement)
        {
            foreach (IElement el in rootElement.QuerySelectorAll("script,style,iframe").ToList())
            {
                el.Remove();
            }
        }

        /// <summary>
        /// Removes useless empty elements
        /// </summary>
        /// <param name="rootElement"></param>
        private void RemoveEmptyElements(IElement rootElement)
        {
            bool removed;
            do
            {
                removed = false;

                foreach (IElement el in rootElement.QuerySelectorAll("div,span").ToList())
                {
                    if (el.ChildElementCount == 0 && string.IsNullOrWhiteSpace(el.TextContent))
                    {
                        removed = true;
                        el.Remove();
                    }
                }
            }
            while (removed);
        }

        private void RemoveEmptyTextNodes(IElement element)
        {
            element.ForAllNodes(child =>
            {
                if (child.NodeType == NodeType.Text && string.IsNullOrWhiteSpace(child.TextContent))
                {
                    child.Remove();
                }
            });
        }

        /// <summary>
        /// Limit sequential &lt;br&gt; to only 2
        /// </summary>
        /// <param name="element"></param>
        private void RemoveMultipleBr(IElement element)
        {
            int i = 2;
            while (i < element.ChildNodes.Length)
            {
                if (element.ChildNodes[i].NodeName == "BR"
                    && element.ChildNodes[i - 1].NodeName == "BR"
                    && element.ChildNodes[i - 2].NodeName == "BR")
                {
                    element.RemoveChild(element.ChildNodes[i]);
                    continue;
                }

                // Recursive
                if (element.ChildNodes[i].NodeType == NodeType.Element && element.ChildNodes[i].ChildNodes.Count(x => x.NodeName == "BR") > 2)
                    RemoveMultipleBr(element.ChildNodes[i] as IElement);

                i++;
            }
        }

        /// <summary>
        /// Remove &lt;br&gt; at beginning and end
        /// </summary>
        /// <param name="element"></param>
        private void RemoveBeginEndBr(IElement element)
        {
            int i = 0;
            while (i < element.ChildNodes.Length)
            {
                if (element.ChildNodes[i].NodeName == "BR"
                    && (i == 0 || i == element.ChildNodes.Length-1))
                {
                    element.RemoveChild(element.ChildNodes[i]);
                    continue;
                }

                // Recursive
                if (element.ChildNodes[i].NodeType == NodeType.Element && element.ChildNodes[i].HasChildNodes)
                    RemoveBeginEndBr(element.ChildNodes[i] as IElement);

                i++;
            }
        }

        /// <summary>
        /// Converts text separated with multiple &lt;br&gt; into paragraphs with surrounding &lt;p&gt;
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="element"></param>
        private void CreateParagraphs(IDocument doc, IElement element)
        {
            foreach (var child in element.ChildNodes.ToList())
            {
                if (child.NodeType == NodeType.Text
                    &&
                    (
                        (child.NextSibling?.NodeName == "BR" && child.NextSibling?.NextSibling?.NodeName == "BR")
                        ||
                        (child.PreviousSibling?.NodeName == "BR" && child.PreviousSibling?.PreviousSibling?.NodeName == "BR")
                    )
                )
                {
                    var next = child.NextSibling;
                    var prev = child.PreviousSibling;

                    // Replace text node with <p>
                    ReplaceElementWithParagraph(doc, child);

                    // Remove following <BR>
                    while (next?.NodeName == "BR")
                    {
                        var curr = next;
                        next = next.NextSibling;
                        curr.Remove();
                    }

                    // Remove preceding <BR>
                    while (prev?.NodeName == "BR")
                    {
                        var curr = prev;
                        prev = prev.PreviousSibling;
                        curr.Remove();
                    }

                    // Last element is text? Then make it also a paragraph
                    if( next?.NodeType == NodeType.Text && next.NextSibling?.NodeName != "BR")
                    {
                        ReplaceElementWithParagraph(doc, next);
                    }
                }

                // Recursive
                else if (child.NodeType == NodeType.Element)
                    CreateParagraphs(doc, child as IElement);
            }
        }

        /// <summary>
        /// Replaces an element with &lt;p&gt; element
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static void ReplaceElementWithParagraph(IDocument doc, INode element)
        {
            var text = element.GetInnerText();

            if (string.IsNullOrWhiteSpace(text))
            {
                element.Remove();
            }
            else
            {
                var newEl = doc.CreateElement("P");
                newEl.TextContent = text;
                element.ReplaceWith(newEl);
            }
        }
    }
}
