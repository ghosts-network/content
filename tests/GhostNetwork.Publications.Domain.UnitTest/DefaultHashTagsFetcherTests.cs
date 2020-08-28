using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace GhostNetwork.Publications.Domain.UnitTest
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
            result.Length.Should().Be(3);
            result[0].Should().Be("first");
            result[1].Should().Be("post");
            result[2].Should().Be("awesome");
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
            result.Length.Should().Be(3);
            result[0].Should().Be("first");
            result[1].Should().Be("post");
            result[2].Should().Be("awesome");
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
            result.Length.Should().Be(2);
            result[0].Should().Be("awesome");
            result[1].Should().Be("so_cool");
        }
    }
}
