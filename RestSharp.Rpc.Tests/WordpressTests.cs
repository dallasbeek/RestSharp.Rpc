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
using RestSharp.Rpc.Tests.Unit.Extensions;

namespace RestSharp.Rpc.Tests {

   namespace RestSharp.Rpc.Tests.Unit {

      //[TestFixture(TestName = "Serialization")]
      public class WordpressTests {

         [Test]
         public void WordPressHelloWorld () {
            var rpcClient = new XmlRpcRestClient( "https://wordpress.com/xmlrpc.php" );

            var sayHelloRequest = new XmlRpcRestRequest( "demo.sayHello" );
            sayHelloRequest.AddXmlRpcBody();
            var helloResponse = rpcClient.Execute<RpcResponseValue<string>>( sayHelloRequest );

            Assert.AreEqual( "Hello!", helloResponse.Data.Value );

         }

         [Test]
         public void WordPressAddTwoNumbers () {
            var rpcClient = new XmlRpcRestClient( "https://wordpress.com/xmlrpc.php" );

            var addTwoNumbersRequest = new XmlRpcRestRequest( "demo.addTwoNumbers " );

            addTwoNumbersRequest.AddXmlRpcBody( 100, 88 );
            var addTwoNumbersResponse = rpcClient.Execute<RpcResponseValue<int>>( addTwoNumbersRequest );

            var sum = addTwoNumbersResponse.Data.Value;
            Assert.AreEqual( 188, addTwoNumbersResponse.Data.Value );

         }

         [Test]
         public void WordPressFault () {
            var rpcClient = new XmlRpcRestClient( "https://wordpress.com/xmlrpc.php" );

            var faultRequest = new XmlRpcRestRequest( "demo.fault " );

            faultRequest.AddXmlRpcBody( );

            var response = rpcClient.Execute<RpcResponseValue<string>>( faultRequest );

            Assert.IsInstanceOf( typeof( XmlRpcFaultException ), response.ErrorException );
            Assert.AreEqual( -32601, ( ( XmlRpcFaultException ) response.ErrorException ).FaultCode );

         }

      }

   }
}