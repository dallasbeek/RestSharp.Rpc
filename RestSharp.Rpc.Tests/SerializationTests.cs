using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using NUnit.Framework;
using RestSharp.Rpc.Tests.Unit.Extensions;

namespace RestSharp.Rpc.Tests {

   namespace RestSharp.Rpc.Tests.Unit {

      //[TestFixture(TestName = "Serialization")]
      public class SerializationTests {

         private XmlSchemaSet _schema;

         [OneTimeSetUp]
         public void Setup() {
            string xsd = EmbeddedResource.LoadFile( "xml-rpc.xsd" );
            using (StringReader sr = new StringReader( xsd ) )
            using (XmlReader xr = XmlReader.Create(sr)) {
               _schema = new XmlSchemaSet();
               _schema.Add( null, xr );
            }
         }

         private void ValidateXml(string xml ) {
            XDocument doc = XDocument.Parse( xml );
            doc.Validate( _schema, ( o, e ) => {
               Assert.Fail( e.Message );
            } );
         }

         [Test, Category( "Xml Serialize" )]
         public void SerializeNoArguments () {
            var request = new XmlRpcRestRequest( "some.method" );
            request.AddXmlRpcBody( );

            var requestBody = request.RequestBody();

            Assert.That(
                requestBody,
                Is.Not.Null,
                "The request body parameter could not be found" );

            if ( requestBody == null ) return;
            ValidateXml( requestBody );

            using ( var sr = new StringReader( requestBody ) ) {
               var doc = new XPathDocument( sr );
               var nav = doc.CreateNavigator();

               Assert.That(
                   nav.Select( "//methodCall/params/param" ).Count,
                   Is.EqualTo( 0 ),
                   "There should be 0 parameters" );

            }

         }

         [Test, Category( "Xml Serialize" )]
         public void SerializeOneString () {
            var request = new XmlRpcRestRequest( "some.method" );
            request.AddXmlRpcBody( "hello" );

            Assert
                .That( request
                    .Parameters[1]
                    .Value
                    .ToString()
                    .Contains( "<string>" ) );
            var requestBody = request.RequestBody();

            Assert.That(
                requestBody,
                Is.Not.Null,
                "The request body parameter could not be found" );

            if ( requestBody == null ) return;
            ValidateXml( requestBody );

            using ( var sr = new StringReader( requestBody ) ) {
               var doc = new XPathDocument( sr );
               var nav = doc.CreateNavigator();

               Assert.That(
                   nav.Select( "//methodCall/params/param" ).Count,
                   Is.EqualTo( 1 ),
                   "There should be 1 parameters" );

               Assert.That(
                   nav.Select( "//methodCall/params/param/value/string" ).Count,
                   Is.EqualTo( 1 ),
                   "There should be 1 string parameter" );

            }

         }

         [Test, Category( "Xml Serialize" )]
         public void SerializeOneIntAsI4 () {
            var request = new XmlRpcRestRequest( "some.method" );
            request.AddXmlRpcBody( 35 );

            Assert
                .That( request
                    .Parameters[1]
                    .Value
                    .ToString()
                    .Contains( "<i4>" ) );
            var requestBody = request.RequestBody();

            Assert.That(
                requestBody,
                Is.Not.Null,
                "The request body parameter could not be found" );

            if ( requestBody == null ) return;
            ValidateXml( requestBody );

            using ( var sr = new StringReader( requestBody ) ) {
               var doc = new XPathDocument( sr );
               var nav = doc.CreateNavigator();

               Assert.That(
                   nav.Select( "//methodCall/params/param" ).Count,
                   Is.EqualTo( 1 ),
                   "There should be 1 parameters" );

               Assert.That(
                   nav.Select( "//methodCall/params/param/value/i4" ).Count,
                   Is.EqualTo( 1 ),
                   "There should be 1 i4 parameter" );
            }
         }

         [Test, Category( "Xml Serialize" )]
         public void SerializeOneIntAsInt () {
            var request = new XmlRpcRestRequest( "some.method", true );
            request.AddXmlRpcBody( 35 );

            Assert
                .That( request
                    .Parameters[1]
                    .Value
                    .ToString()
                    .Contains( "<int>" ) );
            var requestBody = request.RequestBody();

            Assert.That(
                requestBody,
                Is.Not.Null,
                "The request body parameter could not be found" );

            if ( requestBody == null ) return;
            ValidateXml( requestBody );

            using ( var sr = new StringReader( requestBody ) ) {
               var doc = new XPathDocument( sr );
               var nav = doc.CreateNavigator();

               Assert.That(
                   nav.Select( "//methodCall/params/param" ).Count,
                   Is.EqualTo( 1 ),
                   "There should be 1 parameters" );

               Assert.That(
                   nav.Select( "//methodCall/params/param/value/int" ).Count,
                   Is.EqualTo( 1 ),
                   "There should be 1 int parameter" );
            }
         }

         [Test, Category( "Xml Serialize" )]
         public void SerializeOneBoolean () {
            var request = new XmlRpcRestRequest( "some.method", true );
            request.AddXmlRpcBody( true );

            Assert
                .That( request
                    .Parameters[1]
                    .Value
                    .ToString()
                    .Contains( "<boolean>" ) );
            var requestBody = request.RequestBody();

            Assert.That(
                requestBody,
                Is.Not.Null,
                "The request body parameter could not be found" );

            if ( requestBody == null ) return;
            ValidateXml( requestBody );

            using ( var sr = new StringReader( requestBody ) ) {
               var doc = new XPathDocument( sr );
               var nav = doc.CreateNavigator();

               Assert.That(
                   nav.Select( "//methodCall/params/param" ).Count,
                   Is.EqualTo( 1 ),
                   "There should be 1 parameters" );

               Assert.That(
                   nav.Select( "//methodCall/params/param/value/boolean" ).Count,
                   Is.EqualTo( 1 ),
                   "There should be 1 boolean parameter" );
            }
         }

         [Test, Category( "Xml Serialize" )]
         public void SerializeOneDateTime () {
            var request = new XmlRpcRestRequest( "some.method", true );
            var date = DateTime.UtcNow;
            request.AddXmlRpcBody( date );

            Assert
                .That( request
                    .Parameters[1]
                    .Value
                    .ToString()
                    .Contains( "<dateTime.iso8601>" ) );
            var requestBody = request.RequestBody();

            Assert.That(
                requestBody,
                Is.Not.Null,
                "The request body parameter could not be found" );

            if ( requestBody == null ) return;
            ValidateXml( requestBody );

            using ( var sr = new StringReader( requestBody ) ) {
               var doc = new XPathDocument( sr );
               var nav = doc.CreateNavigator();

               Assert.That(
                   nav.Select( "//methodCall/params/param" ).Count,
                   Is.EqualTo( 1 ),
                   "There should be 1 parameters" );

               Assert.That(
                   nav.Select( "//methodCall/params/param/value/dateTime.iso8601" ).Count,
                   Is.EqualTo( 1 ),
                   "There should be 1 dateTime.iso8601 parameter" );

               Assert.That(
                   nav.SelectSingleNode( "//methodCall/params/param/value/dateTime.iso8601" ).Value,
                   Is.EqualTo( date.ToString( "yyyyMMdd'T'HH':'mm':'ss" ) ),
                   "Format of datetime is unexpected" );

            }
         }

         [Test, Category( "Xml Serialize" )]
         public void SerializeOneDateTimeAltFormat () {
            var request = new XmlRpcRestRequest( "some.method", true ) {
               DateFormat = "MMddyyyy'T'HH':'mm':'ss"
            };
            var date = DateTime.UtcNow;
            request.AddXmlRpcBody( date );

            Assert
                .That( request
                    .Parameters[1]
                    .Value
                    .ToString()
                    .Contains( "<dateTime.iso8601>" ) );
            var requestBody = request.RequestBody();

            Assert.That(
                requestBody,
                Is.Not.Null,
                "The request body parameter could not be found" );

            if ( requestBody == null ) return;

            using ( var sr = new StringReader( requestBody ) ) {
               var doc = new XPathDocument( sr );
               var nav = doc.CreateNavigator();

               Assert.That(
                   nav.Select( "//methodCall/params/param" ).Count,
                   Is.EqualTo( 1 ),
                   "There should be 1 parameters" );

               Assert.That(
                   nav.Select( "//methodCall/params/param/value/dateTime.iso8601" ).Count,
                   Is.EqualTo( 1 ),
                   "There should be 1 dateTime.iso8601 parameter" );

               Assert.That(
                   nav.SelectSingleNode( "//methodCall/params/param/value/dateTime.iso8601" ).Value,
                   Is.EqualTo( date.ToString( "MMddyyyy'T'HH':'mm':'ss" ) ),
                   "Format of datetime is unexpected" );

            }
         }

         [Test, Category( "Xml Serialize" )]
         public void SerializeOneDouble () {
            var request = new XmlRpcRestRequest( "some.method", true );
            request.AddXmlRpcBody( 33m );

            Assert
                .That( request
                    .Parameters[1]
                    .Value
                    .ToString()
                    .Contains( "<double>" ) );
            var requestBody = request.RequestBody();

            Assert.That(
                requestBody,
                Is.Not.Null,
                "The request body parameter could not be found" );

            if ( requestBody == null ) return;
            ValidateXml( requestBody );

            using ( var sr = new StringReader( requestBody ) ) {
               var doc = new XPathDocument( sr );
               var nav = doc.CreateNavigator();

               Assert.That(
                   nav.Select( "//methodCall/params/param" ).Count,
                   Is.EqualTo( 1 ),
                   "There should be 1 parameters" );

               Assert.That(
                   nav.Select( "//methodCall/params/param/value/double" ).Count,
                   Is.EqualTo( 1 ),
                   "There should be 1 double parameter" );
            }
         }

         [Test, Category( "Xml Serialize" )]
         public void SerializeOneBase64 () {
            var request = new XmlRpcRestRequest( "some.method", true );
            const string fileContent = "some file content goes here";
            var data = Encoding.ASCII.GetBytes( fileContent );
            request.AddXmlRpcBody( data );

            Assert
                .That( request
                    .Parameters[1]
                    .Value
                    .ToString()
                    .Contains( "<base64>" ) );
            var requestBody = request.RequestBody();

            Assert.That(
                requestBody,
                Is.Not.Null,
                "The request body parameter could not be found" );

            if ( requestBody == null ) return;
            ValidateXml( requestBody );

            using ( var sr = new StringReader( requestBody ) ) {
               var doc = new XPathDocument( sr );
               var nav = doc.CreateNavigator();

               Assert.That(
                   nav.Select( "//methodCall/params/param" ).Count,
                   Is.EqualTo( 1 ),
                   "There should be 1 parameters" );

               Assert.That(
                   nav.Select( "//methodCall/params/param/value/base64" ).Count,
                   Is.EqualTo( 1 ),
                   "There should be 1 base64 parameter" );
            }
         }


         [Test, Category( "Xml Serialize" )]
         public void SerializeOneStringArray () {
            var request = new XmlRpcRestRequest( "some.method", true );
            var data = new[]
             {
                "one",
                "two",
                "three"
            };

            request.AddXmlRpcBody( "", data );


            var requestBody = request.RequestBody();

            Assert.That(
                requestBody,
                Is.Not.Null,
                "The request body parameter could not be found" );

            if ( requestBody == null ) return;
            ValidateXml( requestBody );

            using ( var sr = new StringReader( requestBody ) ) {
               var doc = new XPathDocument( sr );
               var nav = doc.CreateNavigator();

               Assert.That(
                  nav.Select( "//methodCall/params/param" ).Count,
                  Is.EqualTo( 2 ),
                  "There should be 2 parameters" );

               Assert.That(
                  nav.Select( "//methodCall/params/param/value/string" ).Count,
                  Is.EqualTo( 1 ),
                  "There should be 1 string parameter" );

               Assert.That(
                  nav.Select( "//methodCall/params/param/value/array" ).Count,
                  Is.EqualTo( 1 ),
                  "There should be 1 array parameter" );

               Assert.That(
                  nav.Select( "//methodCall/params/param/value/array/data/value/string" ).Count,
                  Is.EqualTo( 3 ),
                  "The array should have 3 (string) elements" );
            }
         }


         [Test, Category( "Xml Serialize" )]
         public void SerializeOneStringList () {
            var data = new System.Collections.Generic.List<string> { "one", "two", "three" };

            var request = new XmlRpcRestRequest( "some.method" );
            request.AddXmlRpcBody(
                data );

            var requestBody = request.RequestBody();

            Assert.That(
                requestBody,
                Is.Not.Null,
                "The request body parameter could not be found" );

            if ( requestBody == null ) return;
            ValidateXml( requestBody );

            using ( var sr = new StringReader( requestBody ) ) {
               var doc = new XPathDocument( sr );
               var nav = doc.CreateNavigator();

               Assert.That(
                   nav.Select( "//methodCall/params/param/value/array/data/value" ).Count,
                   Is.EqualTo( 3 ),
                   "There should be 3 parameters in the array" );
            }
         }

         [Test, Category( "Xml Serialize" )]
         public void SerializeOneSimpleStruct () {
            var data = new SerializeSimpleStruct {
               Name = "Superman",
               Age = 33
            };

            var request = new XmlRpcRestRequest( "some.method" );
            request.AddXmlRpcBody(
                data );

            var requestBody = request.RequestBody();

            Assert.That(
                requestBody,
                Is.Not.Null,
                "The request body parameter could not be found" );

            if ( requestBody == null ) return;
            ValidateXml( requestBody );

            using ( var sr = new StringReader( requestBody ) ) {
               var doc = new XPathDocument( sr );
               var nav = doc.CreateNavigator();

               Assert.That(
                   nav.Select( "//methodCall/params/param/value/struct/member" ).Count,
                   Is.EqualTo( 2 ),
                   "There should be 2 members in struct" );

               Assert.That(
                   nav.SelectSingleNode( "//methodCall/params/param/value/struct/member/name" ).Value,
                   Is.EqualTo( "Name" ),
                   "First Member name should be Superman" );
            }
         }

         [Test, Category( "Xml Serialize" )]
         public void SerializeOneSimpleStructOverrides () {
            var data = new SerializeSimpleStrucOverrides {
               Name = "Superman",
               Age = 33
            };

            var request = new XmlRpcRestRequest( "some.method" );
            request.AddXmlRpcBody(
                data );

            var requestBody = request.RequestBody();

            Assert.That(
                requestBody,
                Is.Not.Null,
                "The request body parameter could not be found" );

            if ( requestBody == null ) return;
            ValidateXml( requestBody );

            using ( var sr = new StringReader( requestBody ) ) {
               var doc = new XPathDocument( sr );
               var nav = doc.CreateNavigator();

               Assert.That(
                   nav.Select( "//methodCall/params/param/value/struct/member" ).Count,
                   Is.EqualTo( 2 ),
                   "There should be 2 members in struct" );

               Assert.That(
                  nav.SelectSingleNode( "//methodCall/params/param/value/struct/member/name" ).Value,
                  Is.EqualTo( "A" ),
                  "First Member name should be A" );
            }

         }

         [Test, Category( "Xml Serialize" )]
         public void SerializeOneComplexStruct () {
            var data = new SerializeComplexStruct( true );

            var request = new XmlRpcRestRequest( "some.method" );
            request.AddXmlRpcBody(
                data );

            var requestBody = request.RequestBody();

            Assert.That(
                requestBody,
                Is.Not.Null,
                "The request body parameter could not be found" );

            if ( requestBody == null ) return;
            ValidateXml( requestBody );

            using ( var sr = new StringReader( requestBody ) ) {
               var doc = new XPathDocument( sr );
               var nav = doc.CreateNavigator();

               Assert.That(
                   nav.Select( "//methodCall/params/param" ).Count,
                   Is.EqualTo( 1 ),
                   "There should be 1 param" );

               Assert.That(
                   nav.Select( "//methodCall/params/param/value/struct/member" ).Count,
                   Is.EqualTo( 4 ),
                   "There should be 4 members in struct" );
            }
         }


         [Test, Category( "Xml Serialize" )]
         public void SerializeArrayOfComplexStructs () {
            var data = new List<SerializeComplexStruct>() { new SerializeComplexStruct( true ), new SerializeComplexStruct( true ), new SerializeComplexStruct( true ) } ;

            var request = new XmlRpcRestRequest( "some.method" );
            request.AddXmlRpcBody(
                data );

            var requestBody = request.RequestBody();

            Assert.That(
                requestBody,
                Is.Not.Null,
                "The request body parameter could not be found" );

            if ( requestBody == null ) return;
            ValidateXml( requestBody );

            using ( var sr = new StringReader( requestBody ) ) {
               var doc = new XPathDocument( sr );
               var nav = doc.CreateNavigator();

               Assert.That(
                   nav.Select( "//methodCall/params/param" ).Count,
                   Is.EqualTo( 1 ),
                   "There should be 1 param" );

            }
         }

      }

   }
}