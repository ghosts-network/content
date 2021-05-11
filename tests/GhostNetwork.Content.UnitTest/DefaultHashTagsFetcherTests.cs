using System.Linq;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest
{
    [TestFixture]
    public class DefaultHashTagsFetcherTests
    {
        [Test]
        public void HashTag_Separated_By_Space()
        {
            // Setup
            var fetcher = new DefaultHashTagsFetcher();
            var text = "#first #post #awesome";

            // Act
            var result = fetcher.Fetch(text)
                .ToArray();

            // Assert
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("first", result[0]);
            Assert.AreEqual("post", result[1]);
            Assert.AreEqual("awesome", result[2]);
        }

        [Test]
        public void HashTag_Without_Separation()
        {
            // Setup
            var fetcher = new DefaultHashTagsFetcher();
            var text = "#first#post#awesome";

            // Act
            var result = fetcher.Fetch(text)
                .ToArray();

            // Assert
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("first", result[0]);
            Assert.AreEqual("post", result[1]);
            Assert.AreEqual("awesome", result[2]);
        }

        [Test]
        public void Only_Underscore_And_Alpha()
        {
            // Setup
            var fetcher = new DefaultHashTagsFetcher();
            var text = "#awesome! #so_cool";

            // Act
            var result = fetcher.Fetch(text)
                .ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("awesome", result[0]);
            Assert.AreEqual("so_cool", result[1]);
        }
    }
}
