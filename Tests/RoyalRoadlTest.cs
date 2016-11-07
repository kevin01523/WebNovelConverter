using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using WebNovelConverter.Sources.Models;
using WebNovelConverter.Sources.Websites;

namespace Tests
{
    [TestFixture]
    public class RoyalRoadlTest
    {
        [Test]
        public void RoyalRoadlIndexTest1()
        {
            var source = new RoyalRoadLSource();
            var uri = new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources/RoyalRoadlIndex1.html")).AbsoluteUri;
            var data = source.GetNovelInfoAsync(uri).Result;

            Assert.AreEqual("The Iron Teeth: A Goblin's Tale", data.Title);
            Assert.AreEqual("http://i.imgur.com/lMkj6s8.png", data.CoverUrl);
            TestHelpers.AreEqualWithDiff(data.Description, @"All the nameless goblin slave ever wanted was not to be beaten by his masters too much, and to eat as much as he could shove into his mouth when no one was looking. Instead, he is drugged and whisked away to the far off Iron Teeth Mountains.
 Will he be able to survive there and evolve into something more than a simple goblin? To stay alive he will have to deal with hordes of deadly monsters, fit in among human bandits, and carve a bloody path through the forests of the North.
 However, first he has to get over his crippling fear of trees, and survive in a place where everything considers him to be the perfect size for a quick snack...
 Don't forget to vote every week at TopWebFiction, thanks.");
        }

        [Test]
        public void RoyalRoadlPageTest1()
        {
            var source = new RoyalRoadLSource();
            var uri = new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources/RoyalRoadlPage1.html")).AbsoluteUri;
            var data = source.GetChapterAsync(new ChapterLink()
            {
                Url = uri,
                Name = "Page 1"
            }).Result;

            TestHelpers.AreEqualWithDiff(data.Content, @"<div style=""text-align: center;""><img src=""http://www.ironteethserial.com/wp-content/uploads/2016/04/RRLBanner2.jpg"" border=""0"" alt=""http%3a%2f%2fwww.ironteethserial.com%2fwp-conten...anner2.jpg""></div><p>Blood poured from the man’s wounds onto the cold ground beneath him. He knew he was dying and beyond help now. He choked and gasped as bitter blood clogged his throat but he fought to hold still and at least die with dignity. In his last moments, and in front of all these witnesses, he wasn’t going to go out thrashing around like a fish out water. Burn that!</p><p>He refused to have regrets, even though he had never gotten what he’d wanted out of life. He had done the best he could and died for what he’d believed in! His would be the last laugh anyway.</p><p>He could still feel the inhuman eyes that watched and blazed with hate. The dying man tried to chuckle but all that came out was a weak gurgling cough. The fools had no idea what they’d unleashed! They couldn’t see how the world had changed and turned against them.</p><p>As the man’s vision grew dark, scenes from his past began to play out before him. His last breath rattled through his teeth and he couldn’t help but think back to how it had all began…</p><p>He remembered a great grey drake looming over the rubble of doomed Coroulis amidst the falling snow. The terrible monster flicked its tail out and smashed an attacking guard. Frozen chunks of meat were thrown into the air as the man shattered. The crimson broken pieces fell to the ground and rolled through the snow. The roar of the beast echoed through the city and his home was forever lost...</p><p>He remembered choosing to live his life out in the Deep Green. The forests of the North contained seemingly endless terrors and monsters but also humbling beauty. There, he’d taken up arms for pride and empty vengeance but then continued fighting just because it was all he’d known. His friends had fallen one by one until he was all alone, and then he had even lost her...</p><p>He remembered a nervous green face looking up at him. It was dirty and ugly but for some reason it mattered to him. Instinctively, he clung to the memory for a stubborn second but then it slipped away with the rest and there was only darkness...</p>");
        }
    }
}
