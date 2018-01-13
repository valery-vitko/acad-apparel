using System.Threading;
using NUnit.Framework;

namespace ACAD.Common.Testing
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class Tests
    {
        [Test]
        public void Test_method_should_pass()
        {
            Assert.Pass("Test that should pass did not pass");
        }

        [Test]
        public void Test_method_should_fail()
        {
            Assert.Fail("This test was supposed to fail.");
        }
    }
}
