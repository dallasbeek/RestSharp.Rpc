using System;

namespace RestSharp.Rpc.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class ByOrdinalAttribute : Attribute
    {
        public int Order { get; set; }
    }
}
