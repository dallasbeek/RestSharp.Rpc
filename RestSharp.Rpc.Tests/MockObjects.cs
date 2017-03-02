using System.Collections.Generic;
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

}
