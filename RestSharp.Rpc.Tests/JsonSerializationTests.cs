using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using NUnit.Framework;
using RestSharp.Rpc.Tests.Unit.Extensions;

namespace RestSharp.Rpc.Tests {

   public class JsonSerializationTests {

      [Test, Category( "Json Serialize" )]
      public void SerializeNoArguments () {
         var request = new JsonRpcRestRequest( "some.method" );
         request.AddJsonRpcBody( null );

         var requestBody = request.RequestBody();
         var jss = new JavaScriptSerializer();
         var data = jss.Deserialize<dynamic>( requestBody );

         Assert.AreEqual( 3, data.Count );
         Assert.AreEqual( "some.method", data["method"] );
         Assert.IsFalse( data.ContainsKey( "params" ) );

      }

      [Test, Category( "Json Serialize" )]
      public void SerializeSpecifiyId () {
         var request = new JsonRpcRestRequest( "some.method", "1234" );
         request.AddJsonRpcBody( null );

         var requestBody = request.RequestBody();
         var jss = new JavaScriptSerializer();
         var data = jss.Deserialize<dynamic>( requestBody );

         Assert.AreEqual( 3, data.Count );
         Assert.AreEqual( "1234", data["id"] );
         Assert.IsFalse( data.ContainsKey( "params" ) );
      }

      [Test, Category( "Json Serialize" )]
      public void SerializeOneString () {
         var request = new JsonRpcRestRequest( "some.method" );
         request.AddJsonRpcBody( "hello" );
         var requestBody = request.RequestBody();
         var jss = new JavaScriptSerializer();
         var data = jss.Deserialize<dynamic>( requestBody );

         Assert.AreEqual( 4, data.Count );
         Assert.AreEqual( "some.method", data["method"] );
         Assert.AreEqual( "hello", data["params"] );

      }


      [Test, Category( "Json Serialize" )]
      public void SerializeOneInt () {
         var request = new JsonRpcRestRequest( "some.method" );
         request.AddJsonRpcBody( 44 );
         var requestBody = request.RequestBody();
         var jss = new JavaScriptSerializer();
         var data = jss.Deserialize<dynamic>( requestBody );

         Assert.AreEqual( 4, data.Count );
         Assert.AreEqual( "some.method", data["method"] );
         Assert.AreEqual( 44, data["params"] );

      }

      [Test, Category( "Json Serialize" )]
      public void SerializeOneBoolen () {
         var request = new JsonRpcRestRequest( "some.method" );
         request.AddJsonRpcBody( true );
         var requestBody = request.RequestBody();
         var jss = new JavaScriptSerializer();
         var data = jss.Deserialize<dynamic>( requestBody );

         Assert.AreEqual( 4, data.Count );
         Assert.AreEqual( "some.method", data["method"] );
         Assert.AreEqual( true, data["params"] );

      }


      [Test, Category( "Json Serialize" )]
      public void SerializeOneDateTime () {
         var now = DateTime.Now;
         var request = new JsonRpcRestRequest( "some.method" );
         request.AddJsonRpcBody( now );
         var requestBody = request.RequestBody();
         var jss = new JavaScriptSerializer();
         var data = jss.Deserialize<dynamic>( requestBody );

         Assert.AreEqual( 4, data.Count );
         Assert.AreEqual( "some.method", data["method"] );
         Assert.AreEqual( now, DateTime.Parse( data["params"] ) );

      }

      [Test, Category( "Json Serialize" )]
      public void SerializeOneStringList () {
         var request = new JsonRpcRestRequest( "some.method" );
         request.AddJsonRpcBody( new List<string> {
            "one",
            "two",
            "three"
         } );

         var requestBody = request.RequestBody();
         var jss = new JavaScriptSerializer();
         var data = jss.Deserialize<dynamic>( requestBody );

         Assert.AreEqual( 4, data.Count );
         Assert.AreEqual( "some.method", data["method"] );
         Assert.AreEqual( 3, data["params"].Length );
         Assert.AreEqual( "one", data["params"][0] );

      }

      [Test, Category( "Json Serialize" )]
      public void SerializeOneSimpleStruct () {
         var rdata = new SerializeSimpleStruct {
            Name = "Superman",
            Age = 33
         };

         var request = new JsonRpcRestRequest( "some.method" );
         request.AddJsonRpcBody( rdata );

         var requestBody = request.RequestBody();
         var jss = new JavaScriptSerializer();
         var data = jss.Deserialize<dynamic>( requestBody );

         Assert.AreEqual( 4, data.Count );
         Assert.AreEqual( "some.method", data["method"] );
         Assert.AreEqual( "Superman", data["params"]["Name"] );
         Assert.AreEqual( 33, data["params"]["Age"] );

      }

      [Test, Category( "Json Serialize" )]
      public void SerializeOneComplexStruct () {
         var rdata = new SerializeComplexStruct( true );

         var request = new JsonRpcRestRequest( "some.method" );
         request.AddJsonRpcBody( rdata );


         var requestBody = request.RequestBody();
         var jss = new JavaScriptSerializer();
         var data = jss.Deserialize<dynamic>( requestBody );

         Assert.AreEqual( 4, data.Count );
         Assert.AreEqual( "some.method", data["method"] );
         Assert.AreEqual( "Base", data["params"]["Name"] );
         Assert.AreEqual( 33, data["params"]["Age"] );
         Assert.IsNotNull( data["params"]["SubObject"] );
         Assert.AreEqual( "SubObject", data["params"]["SubObject"]["Name"] );
         Assert.AreEqual( 3, data["params"]["SubObjectList"].Length );

      }

      [Test, Category( "Json Serialize" )]
      public void SerializeArrayOfComplexStructs () {
         var rdata = new List<SerializeComplexStruct>() { new SerializeComplexStruct( true ), new SerializeComplexStruct( true ), new SerializeComplexStruct( true ) };

         var request = new JsonRpcRestRequest( "some.method" );
         request.AddJsonRpcBody( rdata );

         var requestBody = request.RequestBody();
         var jss = new JavaScriptSerializer();
         var data = jss.Deserialize<dynamic>( requestBody );

         Assert.AreEqual( 4, data.Count );
         Assert.AreEqual( "some.method", data["method"] );
         Assert.AreEqual( 3, data["params"].Length );
         Assert.AreEqual( "Base", data["params"][0]["Name"] );
         Assert.AreEqual( 33, data["params"][0]["Age"] );
         Assert.IsNotNull( data["params"][0]["SubObject"] );
         Assert.AreEqual( "SubObject", data["params"][0]["SubObject"]["Name"] );
         Assert.AreEqual( 3, data["params"][0]["SubObjectList"].Length );


      }

   }
}
