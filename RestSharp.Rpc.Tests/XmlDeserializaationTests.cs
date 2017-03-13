using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RestSharp.Deserializers;

namespace RestSharp.Rpc.Tests {

   public class XmlDeSerializationTests {

      [Test, Category( "Xml Deserialize" )]
      public void DeserializeOneString () {
         var response = new RestResponse();
         response.Content = EmbeddedResource.LoadFile( "ResponseData.XmlOneStringResponse.xml" );
         var deSerializer = new XmlRpcDeserializer();
         var data = deSerializer.Deserialize<RpcResponseValue<string>>( response );
         Assert.AreEqual( "Hello World", data.Value );
      }


      [Test, Category( "Xml Deserialize" )]
      public void DeserializeOneBase64 () {
         var response = new RestResponse();
         response.Content = EmbeddedResource.LoadFile( "ResponseData.XmlOneBase64Response.xml" );
         var deSerializer = new XmlRpcDeserializer();
         var data = deSerializer.Deserialize<RpcResponseValue<byte[]>>( response );
         Assert.AreEqual( "some file content goes here", Encoding.ASCII.GetString( data.Value ) );
      }

      [Test, Category( "Xml Deserialize" )]
      public void DeserializeOneDateTime () {
         var response = new RestResponse();
         response.Content = EmbeddedResource.LoadFile( "ResponseData.XmlOneDateTimeResponse.xml" );
         var deSerializer = new XmlRpcDeserializer();
         var data = deSerializer.Deserialize<RpcResponseValue<DateTime>>( response );
         Assert.AreEqual( new DateTime( 2017, 3, 2, 5, 20, 7 ), data.Value );
      }

      [Test, Category( "Xml Deserialize" )]
      public void DeserializeArrayOfString () {
         var response = new RestResponse();
         response.Content = EmbeddedResource.LoadFile( "ResponseData.XmlArrayOfStringResponse.xml" );
         var deSerializer = new XmlRpcDeserializer();
         var data = deSerializer.Deserialize<List<string>>( response );
         Assert.AreEqual( 3, data.Count );
         Assert.AreEqual( "One", data.First() );
      }

      [Test, Category( "Xml Deserialize" )]
      public void DeserializeArrayOfMixed () {
         var response = new RestResponse();
         response.Content = EmbeddedResource.LoadFile( "ResponseData.XmlArrayOfMixedResponse.xml" );
         var deSerializer = new XmlRpcDeserializer();
         var data = deSerializer.Deserialize<List<object>>( response );
         Assert.AreEqual( 6, data.Count );
         Assert.AreEqual( "One", data.First() );
         Assert.AreEqual( 256.256, Convert.ToDecimal( data[4] ) );
         Assert.AreEqual( "Two", data.Last() );
      }

      [Test, Category( "Xml Deserialize" )]
      public void DeserializeArrayOfMixedToObject () {
         var response = new RestResponse();
         response.Content = EmbeddedResource.LoadFile( "ResponseData.XmlArrayOfMixedResponse.xml" );
         var deSerializer = new XmlRpcDeserializer();
         var data = deSerializer.Deserialize<DeSerializeMixedArray>( response );
         Assert.AreEqual( "One", data.FirstString );
         Assert.AreEqual( "Two", data.ASecondString );
         //Assert.AreEqual( 256.256, Convert.ToDecimal( data.Last() ) );
      }

      [Test, Category( "Xml Deserialize" )]
      public void DeserializeSimpleStruct () {
         var response = new RestResponse();
         response.Content = EmbeddedResource.LoadFile( "ResponseData.XmlSimpleStructResponse.xml" );
         var deSerializer = new XmlRpcDeserializer();
         var data = deSerializer.Deserialize<DeSerializeSimpleStruct>( response );
         Assert.AreEqual( "Title", data.title );
         Assert.AreEqual( "My Description", data.description );
         Assert.AreEqual( 45, data.code );
      }

      [Test, Category( "Xml Deserialize" )]
      public void DeserializeSimpleStructOverrides () {
         var response = new RestResponse();
         response.Content = EmbeddedResource.LoadFile( "ResponseData.XmlSimpleStructResponse.xml" );
         var deSerializer = new XmlRpcDeserializer();
         var data = deSerializer.Deserialize<DeSerializeSimpleStructOverrides>( response );
         Assert.AreEqual( "Title", data.OTitle );
         Assert.AreEqual( "My Description", data.ODescription );
         Assert.AreEqual( 45, data.OCode );
      }

      [Test, Category( "Xml Deserialize" )]
      public void DeserializeComplexStruct () {
         var response = new RestResponse();
         response.Content = EmbeddedResource.LoadFile( "ResponseData.XmlComplexStructResponse.xml" );
         var deSerializer = new XmlRpcDeserializer() { DateFormat = "yyyy-MM-dd'T'HH':'mm':'ss" };
         var data = deSerializer.Deserialize<DeSerializeComplexStruct>( response );
         Assert.AreEqual( "Title", data.title );
         Assert.AreEqual( "My Description", data.description );
         Assert.AreEqual( 45, data.code );
         Assert.IsNotNull( data.subobject );
         Assert.AreEqual( 3, data.events.Count );
         Assert.AreEqual( 3, data.events.First().priceLevels.Count );
         Assert.AreEqual( 3, data.events.First().priceLevels.First().priceTypes.Count );

      }


      [Test, Category( "Xml Deserialize" )]
      public void DeserializeSimpleStructArray () {
         var response = new RestResponse();
         response.Content = EmbeddedResource.LoadFile( "ResponseData.XmlSimpleStructArrayResponse.xml" );
         var deSerializer = new XmlRpcDeserializer();
         var data = deSerializer.Deserialize<List<DeSerializeSimpleStruct>>( response );
         Assert.AreEqual( 3, data.Count );
         Assert.AreEqual( "Title", data.First().title );
         Assert.AreEqual( "My Description", data.First().description );
         Assert.AreEqual( 45, data.First().code );
      }

      [Test, Category( "Xml Deserialize" )]
      public void DeserializeComplexStructArray () {
         var response = new RestResponse();
         response.Content = EmbeddedResource.LoadFile( "ResponseData.XmlComplexStructArrayResponse.xml" );
         var deSerializer = new XmlRpcDeserializer() { DateFormat = "yyyy-MM-dd'T'HH':'mm':'ss" };
         var data = deSerializer.Deserialize<List<DeSerializeComplexStruct>>( response );
         Assert.AreEqual( 3, data.Count );
         Assert.AreEqual( "Title", data.First().title );
         Assert.AreEqual( "My Description", data.First().description );
         Assert.AreEqual( 45, data.First().code );
         Assert.IsNotNull( data.First().subobject );
         Assert.AreEqual( 3, data.First().events.Count );
         Assert.AreEqual( 3, data.First().events.First().priceLevels.Count );
         Assert.AreEqual( 3, data.First().events.First().priceLevels.First().priceTypes.Count );
      }

      [Test, Category( "Xml Deserialize" )]
      public void DeserializeArrayOfMixedArray () {
         var response = new RestResponse();
         response.Content = EmbeddedResource.LoadFile( "ResponseData.XmlArrayOfMixedArrayResponse.xml" );
         var deSerializer = new XmlRpcDeserializer();
         var data = deSerializer.Deserialize<List<DeserializeArrayOfMixedArray>>( response );
         Assert.AreEqual( 3, data.Count );
      }
   }
}
