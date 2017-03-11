using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Deserializers;

namespace RestSharp {

   public class JsonRpcRestClient : RestClient {

      public JsonRpcRestClient () : base() {
         AddHandler( "text/xml", new XmlRpcDeserializer() );
      }

      public JsonRpcRestClient ( string baseUrl ) : base( baseUrl ) {
         AddHandler( "text/xml", new XmlRpcDeserializer() );
      }

      public JsonRpcRestClient ( Uri baseUrl ) : base( baseUrl ) {
         AddHandler( "text/xml", new XmlRpcDeserializer() );
      }

   }
}
