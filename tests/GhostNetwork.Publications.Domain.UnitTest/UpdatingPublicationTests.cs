using System;
using NUnit.Framework;

namespace GhostNetwork.Publications.Domain.UnitTest
{
    [TestFixture]
    public class UpdatingPublicationTests
    {
        [Test]
        public void Publication_Not_Updated_Just_After_Creation()
        {
            // Setup
            var publication = Publication.New("text1", new UserInfo(Guid.Empty, string.Empty, null), content => new string[] { });

            // Assert
            Assert.IsFalse(publication.IsUpdated);
        }

        [Test]
        public void Publication_Updated_After_Update()
        {
            // Setup
            var publication = Publication
                .New("text1", new UserInfo(Guid.Empty, string.Empty, null), content => new string[] { })
                .Update("text2", content => new string[] { });

            // Assert
            Assert.IsTrue(publication.IsUpdated);
        }
    }
}