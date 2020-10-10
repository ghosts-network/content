using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace GhostNetwork.Publications.Domain.UnitTest
{
    [TestFixture]
    public class UpdatingPublicationTests
    {
        [Test]
        public void Test_Update_Publication()
        {
            // Setup
            var publication = new Publication("1", "text1", DateTimeOffset.MinValue, new List<string>(), DateTimeOffset.Now);

            // Assert
            Assert.IsTrue(publication.IsUpdated);
        }

        [Test]
        public void Test_Update_Publication2()
        {
            // Setup
            var time = DateTimeOffset.Now;
            var publication = new Publication("1", "text1", time, new List<string>(), time);

            // Assert
            Assert.IsFalse(publication.IsUpdated);
        }
    }
}