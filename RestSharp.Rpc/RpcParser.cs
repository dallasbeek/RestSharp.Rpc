using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RestSharp {
   public class RpcParser {


      public static XElement Parse ( XElement data ) {
         var valueElement = data.Element( "params" ).Element( "param" ).Element( "value" );
         return ParseValue( valueElement, "Response" );
      }

      private static XElement ParseValue ( XElement value, string name ) {

         var child = value.Descendants().First();
         var childName = child.Name;

         if ( childName == "string" || childName == "i4" || childName == "int" || childName == "boolean" ||
             childName == "string" || childName == "double" || childName == "iso8601" || childName == "base64" ) {
            return new XElement( name, child.Value );
         }else if ( childName == "array" ) {
            return new XElement( name, ExtractArray( child.Element( "data" ).Elements( "value" ), name == "Response" ? null : name ));
         } else if ( childName == "struct" ) {
            return new XElement( name, ExtractStruct( child.Elements( "member" ) ) );
         }
         return null;
      }

      private static List<XElement> ExtractArray ( IEnumerable<XElement> values, string name ) {
         return values.Select( p => ParseValue( p, name ?? p.Descendants().First().Name.ToString() ) ).ToList();
      }

      private static List<XElement> ExtractStruct ( IEnumerable<XElement> members ) {
         return members.Select( p => ExtractMember( p )).ToList();
      }

      private static XElement ExtractMember ( XElement member ) {
         return ParseValue( member.Element( "value" ), (string) member.Element( "name" ) );
      }


   }
}
