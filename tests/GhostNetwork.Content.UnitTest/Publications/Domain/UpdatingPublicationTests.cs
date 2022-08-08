using System;
using GhostNetwork.Content.Publications;
using NUnit.Framework;

namespace GhostNetwork.Content.UnitTest.Publications.Domain
{
    [TestFixture]
    public class UpdatingPublicationTests
    {
        [Test]
        public void Publication_Not_Updated_Just_After_Creation()
        {
            // Setup
            var publication = Publication.New("text1", new UserInfo(Guid.Empty, string.Empty, null), content => Array.Empty<string>());

            // Assert
            Assert.IsFalse(publication.IsUpdated);
        }

        [Test]
        public void Publication_Updated_After_Update()
        {
            // Setup
            var publication = Publication
                .New("text1", new UserInfo(Guid.Empty, string.Empty, null), content => Array.Empty<string>())
                .Update("text2", content => Array.Empty<string>());

            // Assert
            Assert.IsTrue(publication.IsUpdated);
        }
    }
}