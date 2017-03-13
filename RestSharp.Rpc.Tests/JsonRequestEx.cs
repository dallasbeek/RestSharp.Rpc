using System.Linq;

namespace RestSharp.Rpc.Tests.Unit.Extensions {
   public static class JsonRpcRestRequestExtensions {
      public static string RequestBody ( this JsonRpcRestRequest request ) {
         var requestBody = request
             .Parameters
             .SingleOrDefault( x => x.Type == ParameterType.RequestBody )
            ?.Value
            ?.ToString();
         return requestBody;
      }
   }
}
