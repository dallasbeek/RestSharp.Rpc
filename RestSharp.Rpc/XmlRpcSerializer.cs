using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using RestSharp.Extensions;

namespace RestSharp.Serializers {

   public class XmlRpcSerializer : ISerializer {

      private string _methodName;

      public XmlRpcSerializer ( string methodName, bool useIntTag ) {
         ContentType = "text/xml";
         DateFormat = "yyyyMMdd'T'HH':'mm':'ss";
         _methodName = methodName;
         UseIntTag = useIntTag;
      }

      public string ContentType { get; set; }

      public string DateFormat { get; set; }

      public string Namespace { get; set; }

      public string RootElement { get; set; }

      public bool UseIntTag { get; set; }


      public string Serialize ( object obj ) {

         var mCallXml =
          new XElement( "methodCall",
             new XElement( "methodName", _methodName )
          );

         SerializeArguments( mCallXml, obj );

         return "<?xml version=\"1.0\"?>\r\n" + new XDocument( mCallXml ).ToString();
      }

      private void SerializeArguments ( XContainer container, object obj ) {
         var parameters = obj as object[];

         if ( parameters == null || parameters.Length == 0 ) {
            return;
         }

         var paramsElement = new XElement( "params" );
         foreach ( var param in parameters ) {
            Type propType = param.GetType();
#if !WINDOWS_UWP
            if ( propType.IsPrimitive || propType.IsValueType || propType == typeof( string ) )
#else
                if (propType.GetTypeInfo().IsPrimitive || propType.GetTypeInfo().IsValueType || propType == typeof(string))
#endif
                {
               SerializeScaler( paramsElement, param );
            } else if ( propType is IList ) {
               SerializeArray( paramsElement, ( IList ) param );
            } else {
               SerializeStruct( paramsElement, param );
            }
         }
         container.Add( paramsElement );
      }

      private void SerializeScaler ( XContainer root, object obj ) {
         root.Add( new XElement( "param", SerializeValue( obj ) ) );
      }

      private void SerializeStruct ( XContainer root, object obj ) {
         Type objType = obj.GetType();
         IEnumerable<PropertyInfo> props = from p in objType.GetProperties()
                                           let indexAttribute = p.GetAttribute<SerializeAsAttribute>()
                                           where p.CanRead && p.CanWrite
                                           orderby indexAttribute == null
                                               ? int.MaxValue
                                               : indexAttribute.Index
                                           select p;
         SerializeAsAttribute globalOptions = objType.GetAttribute<SerializeAsAttribute>();

         var structElement = new XElement( "struct" );

         foreach ( PropertyInfo prop in props ) {
            string name = prop.Name;
            object rawValue = prop.GetValue( obj, null );

            //if ( rawValue == null ) {
            //   continue;
            //}

            Type propType = prop.PropertyType;
            SerializeAsAttribute settings = prop.GetAttribute<SerializeAsAttribute>();

            if ( settings != null ) {
               name = settings.Name.HasValue()
                   ? settings.Name
                   : name;
            }

            SerializeAsAttribute options = prop.GetAttribute<SerializeAsAttribute>();

            if ( options != null ) {
               name = options.TransformName( name );
            } else if ( globalOptions != null ) {
               name = globalOptions.TransformName( name );
            }

            XElement element = new XElement( "member", new XElement( "name", name ) );
#if !WINDOWS_UWP
            if ( propType.IsPrimitive || propType.IsValueType || propType == typeof( string ) )
#else
                if (propType.GetTypeInfo().IsPrimitive || propType.GetTypeInfo().IsValueType || propType == typeof(string))
#endif
                {

               element.Add( SerializeValue( rawValue ) );
            } else if ( rawValue is IList ) {
               string itemTypeName = "";

               foreach ( object item in ( IList ) rawValue ) {
                  if ( itemTypeName == "" ) {
                     Type type = item.GetType();
                     SerializeAsAttribute setting = type.GetAttribute<SerializeAsAttribute>();

                     itemTypeName = setting != null && setting.Name.HasValue()
                         ? setting.Name
                         : type.Name;
                  }

                  XElement instance = new XElement( itemTypeName );

                  SerializeStruct( instance, item );
                  element.Add( instance );
               }
            } else {
               SerializeStruct( element, rawValue );
            }

            structElement.Add( element );
         }
         root.Add( new XElement( "param", new XElement( "value", structElement ) ) );
      }

      private void SerializeArray ( XContainer root, IList obj ) {

         //string itemTypeName = "";

         var data = new XElement( "data" );
         foreach ( object item in ( IList ) obj ) {
            var itemType = item.GetType();
#if !WINDOWS_UWP
            if ( itemType.IsPrimitive || itemType.IsValueType || itemType == typeof( string ) )
#else
                if (propType.GetTypeInfo().IsPrimitive || propType.GetTypeInfo().IsValueType || propType == typeof(string))
#endif
                {

               data.Add( SerializeValue( item ) );
            } else if ( itemType is IList ) {

            } else {
               SerializeStruct( data, item );
               //data.Add( SerializeValue( item ) );

               //if ( itemTypeName == "" ) {
               //   Type type = item.GetType();
               //   SerializeAsAttribute setting = type.GetAttribute<SerializeAsAttribute>();

               //   itemTypeName = setting != null && setting.Name.HasValue()
               //       ? setting.Name
               //       : type.Name;
               //}

               //XElement instance = new XElement( itemTypeName );

               //SerializeStruct( instance, item );
               //data.Add( instance );
            }
         }
         root.Add( new XElement( "array", data ) );

      }

      private static XElement SerializeValue ( object obj ) {
         var type = obj.GetType();

         XElement value;
         if ( obj is bool ) {
            value = new XElement( "boolean", ( ( bool ) obj ) == true ? 1 : 0 );
         } else if ( obj is DateTime ) {
            value = new XElement( "dateTime.iso8601", ( ( DateTime ) obj ).ToString( "yyyyMMdd'T'HH':'mm':'ss", CultureInfo.InvariantCulture ) );
         } else if ( obj is string ) {
            value = new XElement( "string", obj );
         } else if ( IsNumericInt( obj ) ) {
            value = new XElement( "i4", SerializeNumber( obj ) );
         } else if ( IsNumericDouble( obj ) ) {
            value = new XElement( "double", SerializeNumber( obj ) );
         } else if ( obj is byte[] ) {
            value = new XElement( "base64", obj );
         } else {
            value = new XElement( "string", obj );
         }

         return new XElement( "value", value );
      }

      private static bool IsNumericInt ( object value ) {
         if ( value is sbyte ) {
            return true;
         }

         if ( value is byte ) {
            return true;
         }

         if ( value is short ) {
            return true;
         }

         if ( value is ushort ) {
            return true;
         }

         if ( value is int ) {
            return true;
         }

         if ( value is uint ) {
            return true;
         }

         if ( value is long ) {
            return true;
         }

         if ( value is ulong ) {
            return true;
         }

         return false;
      }

      private static bool IsNumericDouble ( object value ) {
         if ( value is float ) {
            return true;
         }

         if ( value is double ) {
            return true;
         }

         if ( value is decimal ) {
            return true;
         }

         return false;
      }


      private static string SerializeNumber ( object number ) {
         if ( number is long ) {
            return ( ( long ) number ).ToString( CultureInfo.InvariantCulture );
         }

         if ( number is ulong ) {
            return ( ( ulong ) number ).ToString( CultureInfo.InvariantCulture );
         }

         if ( number is int ) {
            return ( ( int ) number ).ToString( CultureInfo.InvariantCulture );
         }

         if ( number is uint ) {
            return ( ( uint ) number ).ToString( CultureInfo.InvariantCulture );
         }

         if ( number is decimal ) {
            return ( ( decimal ) number ).ToString( CultureInfo.InvariantCulture );
         }

         if ( number is float ) {
            return ( ( float ) number ).ToString( CultureInfo.InvariantCulture );
         }

         return ( Convert.ToDouble( number, CultureInfo.InvariantCulture )
                        .ToString( "r", CultureInfo.InvariantCulture ) );
      }

      /// <summary>
      /// Determines if a given object is numeric in any way
      /// (can be integer, double, null, etc).
      /// </summary>
      private static bool IsNumeric ( object value ) {
         if ( value is sbyte ) {
            return true;
         }

         if ( value is byte ) {
            return true;
         }

         if ( value is short ) {
            return true;
         }

         if ( value is ushort ) {
            return true;
         }

         if ( value is int ) {
            return true;
         }

         if ( value is uint ) {
            return true;
         }

         if ( value is long ) {
            return true;
         }

         if ( value is ulong ) {
            return true;
         }

         if ( value is float ) {
            return true;
         }

         if ( value is double ) {
            return true;
         }

         if ( value is decimal ) {
            return true;
         }

         return false;
      }
   }

}
