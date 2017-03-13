using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using RestSharp.Extensions;

namespace RestSharp.Serializers {

   public class XmlRpcSerializer : ISerializer {


      public XmlRpcSerializer ( string methodName, bool useIntTag ) {
         ContentType = "text/xml";
         DateFormat = "yyyyMMdd'T'HH':'mm':'ss";
         MethodName = methodName;
         UseIntTag = useIntTag;
      }

      public string ContentType { get; set; }

      public string DateFormat { get; set; }

      public string Namespace { get; set; }

      public string RootElement { get; set; }

      public string MethodName { get; set; }

      public bool UseIntTag { get; set; }


      public string Serialize ( object obj ) {
         return "<?xml version=\"1.0\"?>\r\n" + SerializeMethodCall( obj ).ToString();
      }

      private XDocument SerializeMethodCall ( object obj ) {
         var methodCallElement =
             new XElement( "methodCall",
                new XElement( "methodName", MethodName )
             );
         methodCallElement.Add( SerializeParam( obj ) );
         return new XDocument( methodCallElement );
      }

      private XElement SerializeParam ( object obj ) {
         var parameters = obj as object[];

         if ( parameters == null || parameters.Length == 0 ) {
            return null;
         }
         var paramsElement = new XElement( "params" );
         foreach ( var param in parameters ) {
            paramsElement.Add( SerializeParamType( param ) );
         }
         return paramsElement;
      }

      private XElement SerializeParamType ( object obj ) {
         return new XElement( "param", SerializeValueType( obj ) );
      }

      private XElement SerializeValueType ( object obj ) {
         Type propType = obj.GetType();
         if ( propType.IsPrimitive || propType.IsValueType || propType == typeof( string ) || propType == typeof( byte[] ) ) {
            //if (propType.GetTypeInfo().IsPrimitive || propType.GetTypeInfo().IsValueType || propType == typeof(string) ||  propType == typeof( byte[] ))

            return new XElement( "value", SerializeValue( obj ) );
         } else if ( propType.Name == "IList`1" || propType.GetInterface( "IList`1" ) != null ) {
            return new XElement( "value", SerializeArrayType( ( IList ) obj ) );
         } else {
            return new XElement( "value", SerializeStructType( obj ) );
         }
      }

      private XElement SerializeValue ( object obj ) {
         var type = obj.GetType();

         if ( obj is bool ) {
            return new XElement( "boolean", ( ( bool ) obj ) == true ? 1 : 0 );
         } else if ( obj is DateTime ) {
            return new XElement( "dateTime.iso8601", ( ( DateTime ) obj ).ToString( DateFormat, CultureInfo.InvariantCulture ) );
         } else if ( obj is string ) {
            return new XElement( "string", obj );
         } else if ( IsNumericInt( obj ) ) {
            return new XElement( UseIntTag ? "int" : "i4", SerializeNumber( obj ) );
         } else if ( IsNumericDouble( obj ) ) {
            return new XElement( "double", SerializeNumber( obj ) );
         } else if ( obj is byte[] ) {
            var data = obj as byte[];
            return new XElement( "base64", Convert.ToBase64String( data ) );
         } else {
            return new XElement( "string", obj );
         }
      }

      private XElement SerializeStructType ( object obj ) {
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
            object rawValue = prop.GetValue( obj, null );

            if ( rawValue == null ) {
               continue;
            }
            structElement.Add( SerializeMemberType( prop, globalOptions, rawValue ) );

         }
         return structElement;
      }

      private XElement SerializeMemberType ( PropertyInfo prop, SerializeAsAttribute globalOptions, object obj ) {
         string name = prop.Name;
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

         return new XElement( "member", new XElement( "name", name ), SerializeValueType( obj ) );

      }

      private XElement SerializeArrayType ( IList obj ) {

         var data = new XElement( "data" );
         foreach ( object item in ( IList ) obj ) {
            data.Add( SerializeValueType( item ) );
         }
         return new XElement( "array", data );
      }

      //      private void SerializeArguments ( XContainer container, object obj ) {
      //         var parameters = obj as object[];

      //         if ( parameters == null || parameters.Length == 0 ) {
      //            return;
      //         }

      //         var paramsElement = new XElement( "params" );
      //         foreach ( var param in parameters ) {
      //            var paramElement = new XElement( "param" );
      //            var valueElement = new XElement( "value" );
      //            Type propType = param.GetType();
      //#if !WINDOWS_UWP
      //            if ( propType.IsPrimitive || propType.IsValueType || propType == typeof( string ) || propType == typeof( byte[] ) )
      //#else
      //                if (propType.GetTypeInfo().IsPrimitive || propType.GetTypeInfo().IsValueType || propType == typeof(string) ||  propType == typeof( byte[] ))
      //#endif
      //                {
      //               paramElement.Add( SerializeValue( param ) );
      //            } else if ( propType.Name == "IList`1" || propType.GetInterface( "IList`1" ) != null ) {
      //               SerializeArray( valueElement, ( IList ) param );
      //               paramElement.Add( valueElement );
      //            } else {
      //               SerializeStruct( valueElement, param );
      //               paramElement.Add( valueElement );
      //            }
      //            paramsElement.Add( paramElement );
      //         }
      //         container.Add( paramsElement );
      //      }

      //      private void SerializeStruct ( XContainer root, object obj ) {
      //         Type objType = obj.GetType();
      //         IEnumerable<PropertyInfo> props = from p in objType.GetProperties()
      //                                           let indexAttribute = p.GetAttribute<SerializeAsAttribute>()
      //                                           where p.CanRead && p.CanWrite
      //                                           orderby indexAttribute == null
      //                                               ? int.MaxValue
      //                                               : indexAttribute.Index
      //                                           select p;
      //         SerializeAsAttribute globalOptions = objType.GetAttribute<SerializeAsAttribute>();

      //         var structElement = new XElement( "struct" );

      //         foreach ( PropertyInfo prop in props ) {
      //            string name = prop.Name;
      //            object rawValue = prop.GetValue( obj, null );

      //            if ( rawValue == null ) {
      //               continue;
      //            }

      //            Type propType = prop.PropertyType;
      //            SerializeAsAttribute settings = prop.GetAttribute<SerializeAsAttribute>();

      //            if ( settings != null ) {
      //               name = settings.Name.HasValue()
      //                   ? settings.Name
      //                   : name;
      //            }

      //            SerializeAsAttribute options = prop.GetAttribute<SerializeAsAttribute>();

      //            if ( options != null ) {
      //               name = options.TransformName( name );
      //            } else if ( globalOptions != null ) {
      //               name = globalOptions.TransformName( name );
      //            }

      //            XElement memberElement = new XElement( "member", new XElement( "name", name ) );
      //            var valueElement = new XElement( "value" );

      //#if !WINDOWS_UWP
      //            if ( propType.IsPrimitive || propType.IsValueType || propType == typeof( string ) || propType == typeof( byte[] ) )
      //#else
      //                if (propType.GetTypeInfo().IsPrimitive || propType.GetTypeInfo().IsValueType || propType == typeof(string) ||  propType == typeof( byte[] ))
      //#endif
      //                {

      //               memberElement.Add( SerializeValue( rawValue ) );
      //            } else if ( propType.Name == "IList`1" || propType.GetInterface( "IList`1" ) != null ) {
      //               SerializeArray( valueElement, ( IList ) rawValue );
      //               memberElement.Add( valueElement );
      //            } else {

      //               SerializeStruct( valueElement, rawValue );
      //               memberElement.Add( valueElement );
      //            }

      //            structElement.Add( memberElement );
      //         }
      //         root.Add( structElement );
      //      }

      //      private void SerializeArray ( XContainer root, IList obj ) {

      //         var data = new XElement( "data" );
      //         foreach ( object item in ( IList ) obj ) {
      //            var valueElement = new XElement( "value" );
      //            var propType = item.GetType();
      //#if !WINDOWS_UWP
      //            if ( propType.IsPrimitive || propType.IsValueType || propType == typeof( string ) || propType == typeof( byte[] ) )
      //#else
      //                if (propType.GetTypeInfo().IsPrimitive || propType.GetTypeInfo().IsValueType || propType == typeof(string) ||  propType == typeof( byte[] ))
      //#endif
      //                {

      //               data.Add( SerializeValue( item ) );
      //            } else if ( propType is IList ) {

      //            } else {
      //               SerializeStruct( valueElement, item );
      //               data.Add( valueElement );
      //            }
      //         }
      //         root.Add( new XElement( "array", data ) );

      //      }

      //private XElement SerializeValue ( object obj ) {
      //   var type = obj.GetType();

      //   XElement value;
      //   if ( obj is bool ) {
      //      value = new XElement( "boolean", ( ( bool ) obj ) == true ? 1 : 0 );
      //   } else if ( obj is DateTime ) {
      //      value = new XElement( "dateTime.iso8601", ( ( DateTime ) obj ).ToString( DateFormat, CultureInfo.InvariantCulture ) );
      //   } else if ( obj is string ) {
      //      value = new XElement( "string", obj );
      //   } else if ( IsNumericInt( obj ) ) {
      //      value = new XElement( UseIntTag ? "int" : "i4", SerializeNumber( obj ) );
      //   } else if ( IsNumericDouble( obj ) ) {
      //      value = new XElement( "double", SerializeNumber( obj ) );
      //   } else if ( obj is byte[] ) {
      //      var data = obj as byte[];
      //      value = new XElement( "base64", Convert.ToBase64String( data ) );
      //   } else {
      //      value = new XElement( "string", obj );
      //   }

      //   return new XElement( "value", value );
      //}

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
