using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

      //public virtual IRestResponse<T> ExecuteRpc<T> ( IRestRequest request ){
      //   return Execute<T>( request );
      //}

   }
}
