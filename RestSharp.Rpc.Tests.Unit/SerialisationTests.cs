using System.IO;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using NUnit.Framework;
using RestSharp.Rpc.Tests.Unit.Extensions;

namespace RestSharp.Rpc.Tests.Unit
{
    public class SerialisationTests
    {
        [Test]
        public void Base64Test()
        {
            const string fileContent = "some file content goes here";
            var data = Encoding.ASCII.GetBytes(fileContent);

            var request = new XmlRpcRestRequest("some.method");
            request.AddXmlRpcBody(
                "",
                data);

            Assert
                .That(request
                    .Parameters[1]
                    .Value
                    .ToString()
                    .Contains("<base64>"));

            var requestBody = request.RequestBody();

            Assert.That(
                requestBody,
                Is.Not.Null,
                "The request body parameter could not be found");

            if (requestBody == null) return;

            using (var sr = new StringReader(requestBody))
            {
                var doc = new XPathDocument(sr);
                var nav = doc.CreateNavigator();

                Assert.That(
                    nav.Select("//methodCall/params/param").Count,
                    Is.EqualTo(2),
                    "There should be 2 parameters");

                Assert.That(
                    nav.Select("//methodCall/params/param/value/string").Count,
                    Is.EqualTo(1),
                    "There should be 1 string parameter");

                Assert.That(
                    nav.Select("//methodCall/params/param/value/base64").Count,
                    Is.EqualTo(1),
                    "There should be 1 base64 parameter");
            }
        }

        [Test]
        public void ListTest()
        {
            var data = new[]
            {
                "one",
                "two",
                "three"
            };

            var request = new XmlRpcRestRequest("some.method");
            request.AddXmlRpcBody(
                "",
                data);

            var requestBody = request.RequestBody();

            Assert.That(
                requestBody,
                Is.Not.Null,
                "The request body parameter could not be found");

            if (requestBody == null) return;

            using (var sr = new StringReader(requestBody))
            {
                var doc = new XPathDocument(sr);
                var nav = doc.CreateNavigator();

                Assert.That(
                    nav.Select("//methodCall/params/param").Count,
                    Is.EqualTo(2),
                    "There should be 2 parameters");

                Assert.That(
                    nav.Select("//methodCall/params/param/value/string").Count,
                    Is.EqualTo(1),
                    "There should be 1 string parameter");

                Assert.That(
                    nav.Select("//methodCall/params/param/value/array").Count,
                    Is.EqualTo(1),
                    "There should be 1 array parameter");

                Assert.That(
                    nav.Select("//methodCall/params/param/value/array/data/value/string").Count,
                    Is.EqualTo(3),
                    "The array should have 3 (string) elements");
            }
        }
    }
}
