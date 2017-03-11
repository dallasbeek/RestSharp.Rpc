using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace RestSharp.Rpc.Tests {
   public class JsonRpcTests {

      [Test, Category( "Json Server" )]
      public void WordPressHelloWorld () {
         var rpcClient = new XmlRpcRestClient( "https://wordpress.com/xmlrpc.php" );

         var sayHelloRequest = new XmlRpcRestRequest( "demo.sayHello" );
         sayHelloRequest.AddXmlRpcBody();
         var helloResponse = rpcClient.Execute<RpcResponseValue<string>>( sayHelloRequest );

         Assert.AreEqual( "Hello!", helloResponse.Data.Value );

      }


   }
}
