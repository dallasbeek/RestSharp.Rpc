using System.Linq;

namespace RestSharp.Rpc.Tests.Unit.Extensions
{
    public static class XmlRpcRestRequestExtensions
    {
        public static string RequestBody(this XmlRpcRestRequest request)
        {
            var requestBody = request
                .Parameters
                .SingleOrDefault(x => x.Type == ParameterType.RequestBody)
               ?.Value
               ?.ToString();
            return requestBody;
        }
    }
}
