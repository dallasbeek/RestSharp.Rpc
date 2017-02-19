using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Serializers;

namespace RestSharp {

   public class XmlRpcRestRequest : RestRequest {

      public XmlRpcRestRequest ( string methodName ) : base( Method.POST ) {
         Initialize( methodName );
      }

      public XmlRpcRestRequest ( string resource, string methodName ) : base( resource, Method.POST ) {
         Initialize( methodName );
      }

      public XmlRpcRestRequest ( Uri resource, string methodName ) : base( resource, Method.POST ) {
         Initialize( methodName );
      }

      public XmlRpcRestRequest ( string methodName, bool useIntTag ) : base( Method.POST ) {
         Initialize( methodName, useIntTag );
      }

      public XmlRpcRestRequest ( string resource, string methodName, bool useIntTag ) : base( resource, Method.POST ) {
         Initialize( methodName, useIntTag );
      }

      public XmlRpcRestRequest ( Uri resource, string methodName, bool useIntTag ) : base( resource, Method.POST ) {
         Initialize( methodName, useIntTag );
      }

      private void Initialize ( string methodName, bool useIntTag = false ) {
         AddHeader( "Accept", string.Empty );
         RequestFormat = DataFormat.Xml;
         XmlSerializer = new XmlRpcSerializer( methodName, useIntTag );
      }

      public IRestRequest AddXmlRpcBody ( params object[] args ) {
         return AddXmlBody( args );
      }
   }
}
