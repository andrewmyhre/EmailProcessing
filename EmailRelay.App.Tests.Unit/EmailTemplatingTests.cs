using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace EmailRelay.App.Tests.Unit
{
    [TestFixture]
    public class EmailTemplatingTests
    {
        [Test]
        public void EmailTemplating_SubjectHasTokens_TokensAreReplaced()
        {
            var template = Helpers.SampleTemplate();


        }
    }
}
