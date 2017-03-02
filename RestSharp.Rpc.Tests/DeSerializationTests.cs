using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using NUnit.Framework;
using RestSharp.Deserializers;
using RestSharp.Rpc.Tests.Unit.Extensions;

namespace RestSharp.Rpc.Tests {

   namespace RestSharp.Rpc.Tests.Unit {

      //[TestFixture(TestName = "Serialization")]
      public class DeSerializationTests {


         [Test]
         public void DeserializeOneString () {
            var response = new RestResponse();
            response.Content = File.ReadAllText( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ) + @"\ResponseData\OneStringResponse.xml" );
            var deSerializer = new XmlRpcDeserializer();
            var data = deSerializer.Deserialize<RpcResponseValue<string>>( response );
            Assert.AreEqual( "Hello World", data.Value );
         }


         [Test]
         public void DeserializeOneBase64 () {
            var response = new RestResponse();
            response.Content = File.ReadAllText( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ) + @"\ResponseData\OneBase64Response.xml" );
            var deSerializer = new XmlRpcDeserializer();
            var data = deSerializer.Deserialize<RpcResponseValue<byte[]>>( response );
            Assert.AreEqual( "some file content goes here", Encoding.ASCII.GetString( data.Value ) );
         }

         [Test]
         public void DeserializeArrayOfString () {
            var response = new RestResponse();
            response.Content = File.ReadAllText( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ) + @"\ResponseData\ArrayOfStringResponse.xml" );
            var deSerializer = new XmlRpcDeserializer();
            var data = deSerializer.Deserialize<List<string>>( response );
            Assert.AreEqual( 3, data.Count );
            Assert.AreEqual( "One", data.First() );
         }

         // need to figure out how to handle this one
         //[Test]
         //public void DeserializeArrayOfMixed () {
         //   var response = new RestResponse();
         //   response.Content = File.ReadAllText( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ) + @"\ResponseData\ArrayOfMixedResponse.xml" );
         //   var deSerializer = new XmlRpcDeserializer();
         //   var data = deSerializer.Deserialize<List<object>>( response );
         //   Assert.AreEqual( 5, data.Count );
         //   Assert.AreEqual( "One", data.First() );
         //   Assert.AreEqual( 256.256, Convert.ToDecimal( data.Last() ) );
         //}

         [Test]
         public void DeserializeSimpleStruct () {
            var response = new RestResponse();
            response.Content = File.ReadAllText( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ) + @"\ResponseData\SimpleStruct.xml" );
            var deSerializer = new XmlRpcDeserializer();
            var data = deSerializer.Deserialize<DeSerializeSimpleStruct>( response );
            Assert.AreEqual( "Title", data.title );
            Assert.AreEqual( "My Description", data.description );
            Assert.AreEqual( 45, data.code );
         }

         [Test]
         public void DeserializeSimpleStructOverrides () {
            var response = new RestResponse();
            response.Content = File.ReadAllText( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ) + @"\ResponseData\SimpleStruct.xml" );
            var deSerializer = new XmlRpcDeserializer();
            var data = deSerializer.Deserialize<DeSerializeSimpleStructOverrides>( response );
            Assert.AreEqual( "Title", data.OTitle );
            Assert.AreEqual( "My Description", data.ODescription );
            Assert.AreEqual( 45, data.OCode );
         }

      }

   }
}