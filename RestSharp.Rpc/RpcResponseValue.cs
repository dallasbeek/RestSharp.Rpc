using System;

namespace RestSharp {

   public class RpcResponseValue<T> where T : IConvertible {
      public T Value { get; set; }
   }

}
