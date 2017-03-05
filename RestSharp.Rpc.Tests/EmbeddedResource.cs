using System.IO;
using System.Reflection;

namespace RestSharp.Rpc.Tests
{
    public static class EmbeddedResource
    {
          public static string LoadFile(string name) {
              Assembly a = Assembly.GetExecutingAssembly();
              using (Stream s = a.GetManifestResourceStream("RestSharp.Rpc.Tests." + name ) ) {
                  using (StreamReader sr = new StreamReader( s )) {
                      return sr.ReadToEnd();
                  }
              }
          }
    }
}
