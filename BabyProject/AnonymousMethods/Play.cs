using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BabyProject.AnonymousMethods
{
    [TestFixture]
    public class Play
    {
        private delegate int SquareDelegate(int val);

        [Test]
        public void C_Sharp_1()
        {
            var doSquare = new SquareDelegate(DoSquareMethod);
            Assert.IsTrue(doSquare(3) == 9);
        }

        [Test]
        public void C_Sharp_2()
        {
            SquareDelegate doSquareDelegate = delegate(int val)
            {
                return val*val;
            };

            Assert.IsTrue(doSquareDelegate(3) == 9);
        }

        private static int DoSquareMethod(int val)
        {
            return val*val;
        }
    }
}
