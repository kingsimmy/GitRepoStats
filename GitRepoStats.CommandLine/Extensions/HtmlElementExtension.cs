using HtmlGenerator;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GitRepoStats.CommandLine.Extensions
{
    public static class HtmlElementExtension
    {
        public static HtmlElement WithChildren(this HtmlElement element, params HtmlElement[] children)
        {
            element.WithChildren(new Collection<HtmlElement>(children));
            return element;
        }

        public static HtmlElement WithChildren(this HtmlElement element, List<HtmlElement> children)
        {
            element.WithChildren(new Collection<HtmlElement>(children));
            return element;
        }
    }
}
