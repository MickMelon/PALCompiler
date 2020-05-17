using NUnit.Framework;
using Assessment;

namespace Assessment.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestAssignTypeMismatch()
        {
            Program.Start(@"Programs\semantic-errors\assignTypeMismatch.txt");
            Assert.Pass();
        }
    }
}