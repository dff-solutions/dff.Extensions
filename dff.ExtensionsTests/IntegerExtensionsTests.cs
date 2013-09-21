using System;
using dff.Extensions;
using NUnit.Framework;

namespace dff.ExtensionsTests
{
    public class IntegerExtensionsTests
    {
        [Test]
        public void MinutesAndSeconds()
        {
            var x = 2.Minutes().Add(2.Seconds());
            Assert.AreEqual(x, new TimeSpan(0,0,2,2));
        }
    }
}