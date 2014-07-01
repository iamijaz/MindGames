using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace BabyProject.AnonymousMethods
{
    [TestFixture]
    public class Play
    {
        private delegate int Square(int val);

        [Test]
        public void C_Sharp_1()
        {
            var doSquare = new Square(DoSquareMethod);
            Assert.IsTrue(doSquare(3) == 9);
        }

        private static int DoSquareMethod(int val)
        {
            return val*val;
        }
    }
}
