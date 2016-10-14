using AngleSharp.Dom;
using System.Linq;

namespace WebNovelConverter.Sources.Helpers
{
    public class ContentCleanup
    {
        public string Execute(IDocument doc, IElement element)
        {
            RemoveEmpty(element);
            RemoveMultipleBr(element);
            CreateParagraphs(doc, element);

            return element.InnerHtml;
        }

        /// <summary>
        /// Removes useless empty elements
        /// </summary>
        /// <param name="rootElement"></param>
        private void RemoveEmpty(IElement rootElement)
        {
            foreach (IElement el in rootElement.QuerySelectorAll("div,span"))
            {
                RemoveEmpty(el);

                if (string.IsNullOrWhiteSpace(el.TextContent) && el.ChildElementCount == 0)
                {
                    el.Remove();
                }
            }
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
        /// Converts text separated with multiple &lt;br&gt; into paragraphs with surrounding &lt;p&gt;
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="element"></param>
        private void CreateParagraphs(IDocument doc, IElement element)
        {
            foreach (var child in element.ChildNodes.ToList())
            {
                if (child.NodeType == NodeType.Text && child.NextSibling?.NodeName == "BR" && child.NextSibling?.NextSibling?.NodeName == "BR")
                {
                    var parent = child.ParentElement;

                    // Remove following <BR>
                    parent.RemoveChild(child.NextSibling.NextSibling);
                    parent.RemoveChild(child.NextSibling);

                    // Replace text node with <p>
                    var pEl = ReplaceElementWithParagraph(doc, child);

                    // If following element is also text node, ALSO MAKE IT <p>
                    if (pEl.NextSibling?.NodeType == NodeType.Text)
                    {
                        ReplaceElementWithParagraph(doc, pEl.NextSibling);
                    }
                }

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
        private IElement ReplaceElementWithParagraph(IDocument doc, INode element)
        {
            var newEl = doc.CreateElement("P");
            newEl.TextContent = element.TextContent.Trim();
            element.Parent.ReplaceChild(newEl, element);
            return newEl;
        }
    }
}
