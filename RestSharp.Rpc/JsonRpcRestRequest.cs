using System;
using RestSharp.Serializers;

namespace RestSharp {

   public class JsonRpcRestRequest : RestRequest {

      public JsonRpcRestRequest ( string methodName ) : base( Method.POST ) {
         Initialize( methodName );
      }

      public JsonRpcRestRequest ( string resource, string methodName, string requestId = null ) : base( resource, Method.POST ) {
         Initialize( methodName, requestId );
      }

      public JsonRpcRestRequest ( Uri resource, string methodName ) : base( resource, Method.POST ) {
         Initialize( methodName );
      }

      public JsonRpcRestRequest ( string methodName, string requestId ) : base( Method.POST ) {
         Initialize( methodName, requestId );
      }

      public JsonRpcRestRequest ( Uri resource, string methodName, string requestId ) : base( resource, Method.POST ) {
         Initialize( methodName, requestId );
      }

      private void Initialize ( string methodName, string requestId = null ) {
         AddHeader( "Accept", string.Empty );
         RequestFormat = DataFormat.Xml;
         JsonSerializer = new JsonRpcSerializer( methodName, requestId );
      }

      public IRestRequest AddJsonRpcBody ( object args ) {
         return AddJsonBody( args );
      }

      public new string DateFormat {
         get {
            return base.DateFormat;
         }
         set {
            base.DateFormat = value;
            XmlSerializer.DateFormat = value;
         }
      }
   }
}
