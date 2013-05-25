using NUnit.Framework;
using dff.Extensions;

namespace dff.ExtensionsTests
{
    public class StringExtensionTests
    {
        [Test]
        public void Shorten_String_Works_For_Normal()
        {
            var x = new string('X', 100).ShortenString(10);
            Assert.AreEqual(x, new string('X', 5) + "..." + new string('X', 2));
        }

        [Test]
        public void Shorten_String_Works_For_Length_3()
        {
            var x = new string('X', 3).ShortenString(10);
            Assert.AreEqual(x, x);
        }

        [Test]
        public void Shorten_String_Works_For_Empty_String()
        {
            var x = new string('X', 0).ShortenString(10);
            Assert.AreEqual(x, x);
        }

        [Test]
        public void Shorten_String_Works_For_Null()
        {
            string x;
            x = null;
            x = x.ShortenString(10);
            Assert.AreEqual(x, string.Empty);
        }

        [Test]
        public void Remove_Last_Seperator_Works_For_Null()
        {
            string x = null;
            x = x.RemoveLastSeperator();
            Assert.IsNull(x, x);
        }

        [Test]
        public void Remove_Last_Seperator_Works_For_Empty()
        {
            var x = string.Empty.RemoveLastSeperator();
            Assert.AreEqual(x, string.Empty);
        }

        [Test]
        public void Remove_Last_Seperator_Works_For_Normal()
        {
            var x = (new string('X', 100) + ",").RemoveLastSeperator();
            Assert.AreEqual(new string('X', 100), x);
        }

        [Test]
        public void Remove_Last_Seperator_Works_For_Normal_Wich_Ends_With_Empty_String()
        {
            var x = (new string('X', 100) + ", ").RemoveLastSeperator();
            Assert.AreEqual(new string('X', 100), x);
        }

        [Test]
        public void Remove_Last_Seperator_Works_For_Double_Seperator_Wich_Ends_With_Empty_String()
        {
            var x = (new string('X', 100) + ",, ").RemoveLastSeperator();
            Assert.AreEqual(new string('X', 100), x);
        }
    }
}
