using GhostNetwork.Content.Publications;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GhostNetwork.Content.UnitTest
{
    [TestFixture]
    public class TimeLimitToUpdateValidatorTests
    {
        [Test]
        public async Task TimeLimit_Expired()
        {
            // Setup
            const string content = "some content";
            const int timeLimit = 900;

            var publication = new Publication(
                string.Empty,
                content,
                Enumerable.Empty<string>(),
                author: null,
                DateTimeOffset.UtcNow.AddSeconds(-timeLimit),
                DateTimeOffset.UtcNow.AddSeconds(-timeLimit));

            var validator = new TimeLimitToUpdateValidator(TimeSpan.FromSeconds(timeLimit));

            // Act
            var result = await validator.ValidateAsync(publication);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public async Task TimeLimit_Dont_Expired()
        {
            // Setup
            const string content = "some content";
            const int timeLimit = 900;

            var publication = Publication.New(content, null, f => Enumerable.Empty<string>());

            var validator = new TimeLimitToUpdateValidator(TimeSpan.FromSeconds(timeLimit));

            // Act
            var result = await validator.ValidateAsync(publication);

            // Assert
            Assert.IsTrue(result.Successed);
        }
    }
}
