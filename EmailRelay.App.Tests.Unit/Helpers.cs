using System;
using EmailProcessing;

namespace EmailRelay.App.Tests.Unit
{
    public class Helpers
    {
        public static EmailTemplate SampleTemplate()
        {
            return new EmailTemplate()
                       {
                           From = "test@test.com",
                           Html = "test html {token1}",
                           Text = "test text {token2}",
                           Subject = "test subject {token3}",
                           Tokens = new TokenList {"token1", "token2", "token3"}
                       };
        }
        public static EmailPackage SamplePackage()
        {
            return new EmailPackage()
            {
                From = "test@test.com",
                Html = "test html {token1}",
                Text = "test text {token2}",
                Subject = "test subject {token3}"
            };
        }

    }
}