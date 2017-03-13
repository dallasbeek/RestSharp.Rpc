using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using RestSharp.Deserializers;

namespace RestSharp.Rpc.Tests {

   public class JsonDeserializationTests {

      [Test, Category( "Json Deserialize" )]
      public void DeserializeOneString () {
         var response = new RestResponse();
         response.Content = EmbeddedResource.LoadFile( "ResponseData.JsonOneStringResponse.txt" );
         var deSerializer = new JsonRpcDeserializer();
         var data = deSerializer.Deserialize<string>( response );
         Assert.AreEqual( "Hello World", data );
      }

      [Test, Category( "Json Deserialize" )]
      public void DeserializeArrayOfString () {
         var response = new RestResponse();
         response.Content = EmbeddedResource.LoadFile( "ResponseData.JsonArrayOfStringResponse.txt" );
         var deSerializer = new JsonRpcDeserializer();
         var data = deSerializer.Deserialize<List<string>>( response );
         Assert.AreEqual( 3, data.Count );
         Assert.AreEqual( "One", data.First() );
      }

      [Test, Category( "Json Deserialize" )]
      public void DeserializeSimpleStruct () {
         var response = new RestResponse();
         response.Content = EmbeddedResource.LoadFile( "ResponseData.JsonSimpleStructResponse.txt" );
         var deSerializer = new JsonRpcDeserializer();
         var data = deSerializer.Deserialize<DeSerializeSimpleStruct>( response );
         Assert.AreEqual( "Title", data.title );
         Assert.AreEqual( "My Description", data.description );
         Assert.AreEqual( 45, data.code );
      }

      [Test, Category( "Json Deserialize" )]
      public void DeserializeSimpleStructArray () {
         var response = new RestResponse();
         response.Content = EmbeddedResource.LoadFile( "ResponseData.JsonSimpleStructArrayResponse.txt" );
         var deSerializer = new JsonRpcDeserializer();
         var data = deSerializer.Deserialize<List<DeSerializeSimpleStruct>>( response );
         Assert.AreEqual( 3, data.Count );
         Assert.AreEqual( "Title", data.First().title );
         Assert.AreEqual( "My Description", data.First().description );
         Assert.AreEqual( 45, data.First().code );
      }

      [Test, Category( "Json Deserialize" )]
      public void ErrorResponse () {
         var response = new RestResponse();
         response.Content = EmbeddedResource.LoadFile( "ResponseData.JsonErrorResponse.txt" );
         var deSerializer = new JsonRpcDeserializer();

         try {
            var data = deSerializer.Deserialize<int>( response );
         } catch ( JsonRpcFaultException ex ) {
            Assert.AreEqual( -32601, ex.FaultCode );
            return;
         }
         Assert.Fail( "Exception not thrown" );
      }


   }
}
