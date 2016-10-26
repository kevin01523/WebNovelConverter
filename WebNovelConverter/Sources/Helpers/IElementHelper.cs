using AngleSharp.Dom;
using System;
using System.Linq;

namespace WebNovelConverter.Sources.Helpers
{
    public static class IElementHelper
    {
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
    }
}
