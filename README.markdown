# RestSharp RPC - Serialization and Deserialation for executing XML Rpc Requests

### License: Apache License 2.0

### Usage

```csharp
var rpcClient = new XmlRpcRestClient( "https://wordpress.com/xmlrpc.php" );

// Set the MethodCall
var sayHelloRequest = new XmlRpcRestRequest( "demo.sayHello" );

// Add MethodCallXml
sayHelloRequest.AddXmlRpcBody( );

// Use RpcResponseValue<T> for methods that return value
var helloResponse = rpcClient.Execute<RpcResponseValue<string>>( sayHelloRequest );

var hello = helloResponse.Data.Value;



var rpcClient = new XmlRpcRestClient( "https://wordpress.com/xmlrpc.php" );

// Set the MethodCall
var addTwoNumbersRequest = new XmlRpcRestRequest( "demo.addTwoNumbers " );

// Add arguments to the request
addTwoNumbersRequest.AddXmlRpcBody( 100, 88 );
var addTwoNumbersResponse = rpcClient.Execute<RpcResponseValue<int>>( addTwoNumbersRequest );

var sum = addTwoNumbersResponse.Data.Value;

```

### Faults

A fault response from the server throws an XmlRpcFaultException