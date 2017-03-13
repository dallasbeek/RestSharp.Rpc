using System;
using System.Linq;
using System.Text;

namespace RestSharp.Serializers {

   public class JsonRpcSerializer : ISerializer {


      public JsonRpcSerializer ( string methodName, string id ) {
         ContentType = "text/xml";
         //DateFormat = "yyyyMMdd'T'HH':'mm':'ss";
         MethodName = methodName;
         RequestId = id;
      }

      public JsonRpcSerializer () {
         this.ContentType = "application/json";
      }
      public string DateFormat { get; set; }

      public string RootElement { get; set; }

      public string Namespace { get; set; }

      public string ContentType { get; set; }

      public string MethodName { get; set; }

      public string RequestId { get; set; }

      public string Serialize ( object obj ) {
         return SerializeMethodCall( obj ).ToString();
      }

      private string SerializeMethodCall ( object obj ) {
         var builder = new StringBuilder( 2000 );
         builder.Append( "{" );
         AppendNameValue( builder, "jsonrpc", "2.0" );
         builder.Append( ',' );
         AppendNameValue( builder, "method", MethodName );
         builder.Append( ',' );

         SerializeParam( builder, obj );

         AppendNameValue( builder, "id", RequestId ?? GetRandomRequestId() );
         builder.Append( "}" );
         return builder.ToString();
      }

      private void SerializeParam ( StringBuilder builder, object obj ) {
         //{"jsonrpc": "2.0", "method": "subtract", "params": [23, 42], "id": 2}
         //var parameters = obj as object[];

         if ( obj == null ) {
            return;
         }
         builder.Append( '"' );
         builder.Append( "params" );
         builder.Append( '"' );
         builder.Append( ": " );

         builder.Append( SimpleJson.SerializeObject( obj ) );
         builder.Append( ',' );
      }

      private void AppendNameValue ( StringBuilder builder, string name, string value ) {
         builder.Append( '"' );
         builder.Append( name );
         builder.Append( '"' );
         builder.Append( ": " );
         builder.Append( '"' );
         builder.Append( value );
         builder.Append( '"' );
      }

      private static Random random = new Random();

      private static string GetRandomRequestId () {
         var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
         var result = new string(
             Enumerable.Repeat( chars, 8 )
                       .Select( s => s[random.Next( s.Length )] )
                       .ToArray() );

         return result;
      }
   }

}
