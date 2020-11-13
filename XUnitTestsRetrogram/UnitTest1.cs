using System;
using Xunit;

namespace XUnitTestsRetrogram
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("goto axl.verheul")]
        [InlineData("goto 106884")]
        [InlineData("goto #hastag")]
        [InlineData("login")]
        [InlineData("comment I really like your drawing")]
        [InlineData("comment this is dope! #dope #cool")]
        public void ConvertInput(string input)
        {
            ConvertInput("goto axl.verheul");
        }
    }
}
