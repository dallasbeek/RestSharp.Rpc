using System.Collections.Generic;
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace RestSharp.Rpc.Tests {
   public class SerializeSimpleStruct {
      public string Name { get; set; }
      public int Age { get; set; }
   }

   public class SerializeSimpleStrucOverrides {
      [SerializeAs( Name = "N", Index = 2 )]
      public string Name { get; set; }
      [SerializeAs( Name = "A", Index = 1 )]
      public int Age { get; set; }
   }

   public class SerializeComplexStruct {

      public SerializeComplexStruct (bool isBase = false) {
         Name = isBase ? "Base" : "SubObject";
         Age = 33;
         if ( isBase ) {
            SubObject = new SerializeComplexStruct();
            SubObjectList = new List<SerializeComplexStruct> {
               new SerializeComplexStruct(),
               new SerializeComplexStruct(),
               new SerializeComplexStruct()
            };
         }
      }

      public string Name { get; set; }
      public int Age { get; set; }
      public SerializeComplexStruct SubObject { get; set; }
      public List<SerializeComplexStruct> SubObjectList { get; set; }
   }

   public class DeSerializeSimpleStruct {

      public string title { get; set; }
      public string description { get; set; }
      public int code { get; set; }
   }

   public class DeSerializeSimpleStructOverrides {
      [DeserializeAs( Name ="title")]
      public string OTitle { get; set; }
      [DeserializeAs( Name = "description" )]
      public string ODescription { get; set; }
      [DeserializeAs( Name = "code" )]
      public int OCode { get; set; }
   }

}
