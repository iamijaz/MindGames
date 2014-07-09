using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace BabyProject.Generic
{
    [TestFixture]
    public class GenericTests
    {
        [Test]
        public void M1()
        {
            // with lambda
            var val1 = Get("EurlAxdRedirect1", () => "haha");
            var val2 = Get("EurlAxdRedirect2", () => 420);

            Assert.IsTrue(val1 == "haha");
            Assert.IsTrue(val2 == 420);


            // without lambda
            var val3 = Get("EurlAxdRedirect1", "haha");
            var val4 = Get("EurlAxdRedirect2", 420);

            Assert.IsTrue(val3 == "haha");
            Assert.IsTrue(val4 == 420);
        }

        public T Get<T>(string key, T t=default(T))
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }
            
            var value = ConfigurationManager.AppSettings[key];
            if (value == null)
            {
                return t;
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }
        public T Get<T>(string key, Func<T> defaultValue = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            defaultValue = defaultValue ?? (() => default(T));
            var value = ConfigurationManager.AppSettings[key];
            if (value == null)
            {
                return defaultValue();
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
