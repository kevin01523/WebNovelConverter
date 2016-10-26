using AngleSharp.Dom;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebNovelConverter.Sources.Helpers
{
    public static class IElementHelper
    {
        public static void ForAllNodes(this INode node, Action<INode> action)
        {
            foreach (var child in node.ChildNodes.ToList())
            {
                action(child);

                // Recursive
                if( child.HasChildNodes)
                    ForAllNodes(child, action);
            }
        }

        public static void ForAllElements(this IElement element, Action<IElement> action)
        {
            foreach (var child in element.ChildNodes.Where(x => x.NodeType == NodeType.Element).ToList())
            {
                var childEl = child as IElement;
                action(childEl);

                // Recursive
                ForAllElements(childEl, action);
            }
        }

        public static void ReplaceWith(this INode self, INode node)
        {
            self.Parent.ReplaceChild(node, self);
        }

        public static void Remove(this INode self)
        {
            self.Parent.RemoveChild(self);
        }

        public static string GetInnerText(this INode self)
        {
            return Regex.Replace(self.TextContent.Trim(), "[ ]{2,}", " ");
        }
    }
}
