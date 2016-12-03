using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebNovelConverter
{
    public static class UrlHelper
    {
        public static bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }

        public static string ToAbsoluteUrl(string baseUrl, string relativeUrl)
        {
            if (IsAbsoluteUrl(relativeUrl))
            {
                if (relativeUrl.StartsWith("//"))
                    return new Uri(baseUrl).Scheme + relativeUrl;
                else
                    return relativeUrl;
            }
            
            Uri result;
            if (Uri.TryCreate(new Uri(baseUrl), relativeUrl, out result))
                return result.AbsoluteUri;

            return null;
        }

        public static bool IsOtherDomain(string url, string baseUrl)
        {
            var hrefUri = new Uri(ToAbsoluteUrl(baseUrl, url), UriKind.Absolute);
            var host = hrefUri.Host;
            if (host.StartsWith("www."))
                host = host.Substring("www.".Length);

            var baseHost = new Uri(baseUrl, UriKind.Absolute).Host;
            if (baseHost.StartsWith("www."))
                baseHost = baseHost.Substring("www.".Length);

            return host != baseHost;
        }
    }
}
