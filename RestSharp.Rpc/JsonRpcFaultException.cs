using System;
using System.Runtime.Serialization;

namespace RestSharp {

   [Serializable]
   public class JsonRpcFaultException : ApplicationException {

      private int _faultCode;
      private string _faultString;

      public JsonRpcFaultException ( int faultCode, string faultString )
        : base( "Server returned a fault exception: [" + faultCode.ToString() + "] " + faultString ) {
         _faultCode = faultCode;
         _faultString = faultString;
      }

      protected JsonRpcFaultException ( SerializationInfo info, StreamingContext context ) : base( info, context ) {
         _faultCode = ( int ) info.GetValue( "_faultCode", typeof( int ) );
         _faultString = ( string ) info.GetValue( "_faultString", typeof( string ) );
      }

      public int FaultCode {
         get { return _faultCode; }
      }

      public string FaultString {
         get { return _faultString; }
      }

      public override void GetObjectData ( SerializationInfo info, StreamingContext context ) {
         info.AddValue( "_faultCode", _faultCode );
         info.AddValue( "_faultString", _faultString );
         base.GetObjectData( info, context );
      }

   }
}
