using System;
using RestSharp.Deserializers;

namespace RestSharp {

   public class XmlRpcRestClient : RestClient {

      public XmlRpcRestClient () : base() {
         AddHandler( "text/xml", new XmlRpcDeserializer() );
      }

      public XmlRpcRestClient ( string baseUrl ) : base( baseUrl ) {
         AddHandler( "text/xml", new XmlRpcDeserializer() );
      }

      public XmlRpcRestClient ( Uri baseUrl ) : base( baseUrl ) {
         AddHandler( "text/xml", new XmlRpcDeserializer() );
      }

   }
}
