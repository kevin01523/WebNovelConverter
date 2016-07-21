using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using WebNovelConverter.Sources.Models;

namespace WebNovelConverter.Sources
{
    public class FictionPressSource : FanFictionSource
    {
        public override string BaseUrl => "https://www.fictionpress.com";

        public FictionPressSource() : base("Fictionpress")
        {
        }
    }
}
