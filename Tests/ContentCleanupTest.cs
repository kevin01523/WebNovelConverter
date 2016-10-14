using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebNovelConverter.Sources.Helpers;
using AngleSharp.Parser.Html;
using AngleSharp.Dom;

namespace Tests
{
    [TestClass]
    public class ContentCleanupTest
    {
        private string CreateTest(string content)
        {
            var doc = new HtmlParser().Parse(@"<div class='pagecontent'>" + content + "</div>");
            return new ContentCleanup().Execute(doc, doc.QuerySelector(".pagecontent"));
        }

        [TestMethod]
        public void Test1()
        {
            var result = CreateTest("Hello world<br><br/><BR><div>This is a test</div>");

            Assert.AreEqual(@"<p>Hello world</p><div>This is a test</div>", result);
        }

        [TestMethod]
        public void Test2()
        {
            var result = CreateTest("<div id='a1'><div id='a2'></div>Hello world<br><br>How?</div>");

            Assert.AreEqual("<div id=\"a1\"><p>Hello world</p><p>How?</p></div>", result);
        }
    }
}
