using NUnit.Framework;

namespace RestSharp.Rpc.Tests {

   public class XmlWordpressTests {

      [Test, Category( "Xml Wordpress" )]
      public void WordPressHelloWorld () {
         var rpcClient = new XmlRpcRestClient( "https://wordpress.com/xmlrpc.php" );

         var sayHelloRequest = new XmlRpcRestRequest( "demo.sayHello" );
         sayHelloRequest.AddXmlRpcBody();
         var helloResponse = rpcClient.Execute<RpcResponseValue<string>>( sayHelloRequest );

         Assert.AreEqual( "Hello!", helloResponse.Data.Value );

      }

      [Test, Category( "Xml Wordpress" )]
      public void WordPressAddTwoNumbers () {
         var rpcClient = new XmlRpcRestClient( "https://wordpress.com/xmlrpc.php" );

         var addTwoNumbersRequest = new XmlRpcRestRequest( "demo.addTwoNumbers " );

         addTwoNumbersRequest.AddXmlRpcBody( 100, 88 );
         var addTwoNumbersResponse = rpcClient.Execute<RpcResponseValue<int>>( addTwoNumbersRequest );

         var sum = addTwoNumbersResponse.Data.Value;
         Assert.AreEqual( 188, addTwoNumbersResponse.Data.Value );

      }

      [Test, Category( "Xml Wordpress" )]
      public void WordPressFault () {
         var rpcClient = new XmlRpcRestClient( "https://wordpress.com/xmlrpc.php" );

         var faultRequest = new XmlRpcRestRequest( "demo.fault " );

         faultRequest.AddXmlRpcBody();

         var response = rpcClient.Execute<RpcResponseValue<string>>( faultRequest );

         Assert.IsInstanceOf( typeof( XmlRpcFaultException ), response.ErrorException );
         Assert.AreEqual( -32601, ( ( XmlRpcFaultException ) response.ErrorException ).FaultCode );

      }

   }

}