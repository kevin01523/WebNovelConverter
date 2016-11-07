using WebNovelConverter.Sources.Helpers;
using AngleSharp.Parser.Html;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class ContentCleanupTest
    {
        private string CreateTest(string content)
        {
            var doc = new HtmlParser().Parse(@"<div class='pagecontent'>" + content + "</div>");
            return new ContentCleanup("http://test.nl/").Execute(doc, doc.QuerySelector(".pagecontent"));
        }

        [Test]
        public void ContentCleanupTest1()
        {
            var result = CreateTest("Hello world<br><br/><BR><div>This is a test</div>");

            Assert.AreEqual(@"<p>Hello world</p><div>This is a test</div>", result);
        }

        [Test]
        public void ContentCleanupTest2()
        {
            var result = CreateTest("<div id='a1'><div id='a2'></div>Hello world<br><br>How?</div>");

            Assert.AreEqual("<div id=\"a1\"><p>Hello world</p><p>How?</p></div>", result);
        }

        [Test]
        public void ContentCleanupTest3()
        {
            var result = CreateTest(@"Blood poured from the man’s wounds onto the cold ground beneath him. He knew he was dying and beyond help now. He choked and gasped as bitter blood clogged his throat but he fought to hold still and at least die with dignity. In his last moments, and in front of all these witnesses, he wasn’t going to go out thrashing around like a fish out water. Burn that!
                                <br />
                                <br />He refused to have regrets, even though he had never gotten what he’d wanted out of life. He had done the best he could and died for what he’d believed in! His would be the last laugh anyway.
                                <br />");

            Assert.AreEqual(@"<p>Blood poured from the man’s wounds onto the cold ground beneath him. He knew he was dying and beyond help now. He choked and gasped as bitter blood clogged his throat but he fought to hold still and at least die with dignity. In his last moments, and in front of all these witnesses, he wasn’t going to go out thrashing around like a fish out water. Burn that!</p><p>He refused to have regrets, even though he had never gotten what he’d wanted out of life. He had done the best he could and died for what he’d believed in! His would be the last laugh anyway.</p>", result);
        }

        [Test]
        public void ContentCleanupTestRelativeImg()
        {
            var result = CreateTest("Hello <img src=\"/Smiley.png\"> World");

            Assert.AreEqual("Hello <img src=\"http://test.nl/Smiley.png\"> World", result);
        }
    }
}
