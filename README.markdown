# RestSharp RPC - Serialization and Deserialation for executing XML Rpc Requests

### License: Apache License 2.0

### Usage

```csharp
var rpcClient = new XmlRpcRestClient( "https://wordpress.com/xmlrpc.php" );
var sayHelloRequest = new XmlRpcRestRequest( "demo.sayHello" );

// Add MethodCallXml
sayHelloRequest.AddXmlRpcBody( );
var onSaleInfoResponse = rpcClient.Execute<RpcResponseValue<string>>( sayHelloRequest );

var helloResponse = onSaleInfoResponse.Data.Value;


var rpcClient = new XmlRpcRestClient( "https://wordpress.com/xmlrpc.php" );
var sayHelloRequest = new XmlRpcRestRequest( "demo.addTwoNumbers " );

sayHelloRequest.AddXmlRpcBody(100,88);
var onSaleInfoResponse = rpcClient.Execute<RpcResponseValue<int>>( sayHelloRequest );

var data = onSaleInfoResponse.Data.Value;

```