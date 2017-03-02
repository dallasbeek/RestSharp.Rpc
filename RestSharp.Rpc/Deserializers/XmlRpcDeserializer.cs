using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using RestSharp.Extensions;
using RestSharp;

#if !SILVERLIGHT && !WINDOWS_PHONE
using System.ComponentModel;
#endif

namespace RestSharp.Deserializers {

   public class XmlRpcDeserializer : IDeserializer {

      public XmlRpcDeserializer () {
         this.Culture = CultureInfo.InvariantCulture;
         DateFormat = "yyyyMMdd'T'HH':'mm':'ss";
      }

      public string RootElement { get; set; }

      public string Namespace { get; set; }

      public string DateFormat { get; set; }

      public CultureInfo Culture { get; set; }


      public virtual T Deserialize<T> ( IRestResponse response ) {
         if ( string.IsNullOrEmpty( response.Content ) ) {
            return default( T );
         }

         XDocument rdoc = XDocument.Parse( response.Content );

         if ( rdoc.Root.Element( "fault" ) != null ) {
            throw ParseFault( rdoc.Root.Element( "fault" ) );
         }
         var doc = TransformXml( rdoc.Root );
         XElement root = doc;

         T x = Activator.CreateInstance<T>();
         Type objType = x.GetType();

         if ( objType.IsSubclassOfRawGeneric( typeof( List<> ) ) ) {
            x = ( T ) this.HandleListDerivative( root, objType.Name, objType );
         } else {
            x = ( T ) this.Map( x, root );
         }

         return x;
      }

      private static XmlRpcFaultException ParseFault ( XElement fault ) {
         var faultMembers = fault.Element( "value" ).Element( "struct" ).Elements( "member" );
         int faultCodeValue = -1;
         string faultStringValue = "unknown";

         var faultCodeMember = faultMembers.Where( i => ( string ) i.Element( "name" ) == "faultCode" ).SingleOrDefault();
         if ( faultCodeMember != null ) {
            if ( faultCodeMember.Element( "value" ).Element( "int" ) != null ) {
               faultCodeValue = ( int ) faultCodeMember.Element( "value" ).Element( "int" );
            }
            if ( faultCodeMember.Element( "value" ).Element( "i4" ) != null ) {
               faultCodeValue = ( int ) faultCodeMember.Element( "value" ).Element( "i4" );
            }
         }

         var faultStringMember = faultMembers.Where( i => ( string ) i.Element( "name" ) == "faultString" ).SingleOrDefault();
         if ( faultStringMember != null ) {
            if ( faultStringMember.Element( "value" ).Element( "string" ) != null ) {
               faultStringValue = ( string ) faultStringMember.Element( "value" ).Element( "string" );
            }
         }

         return new XmlRpcFaultException( faultCodeValue, faultStringValue );
      }

      protected virtual object Map ( object x, XElement root ) {

         Type objType = x.GetType();

         PropertyInfo[] props = objType.GetProperties();

         foreach ( PropertyInfo prop in props ) {
            Type type = prop.PropertyType;

#if !WINDOWS_UWP
            bool typeIsPublic = type.IsPublic || type.IsNestedPublic;
#else
                bool typeIsPublic = type.GetTypeInfo().IsPublic || type.GetTypeInfo().IsNestedPublic;
#endif

            if ( !typeIsPublic || !prop.CanWrite ) {
               continue;
            }

            XName name;
#if !WINDOWS_UWP
            object[] attributes = prop.GetCustomAttributes( typeof( DeserializeAsAttribute ), false );

            if ( attributes.Length > 0 )
#else
                IEnumerable<Attribute> attributes = prop.GetCustomAttributes(typeof(DeserializeAsAttribute), false);                

                if (attributes.Count() > 0)
#endif
                {
               DeserializeAsAttribute attribute = ( DeserializeAsAttribute ) attributes.First();

               name = attribute.Name.AsNamespaced( this.Namespace );
            } else {
               name = prop.Name.AsNamespaced( this.Namespace );
            }

            object value = this.GetValueFromXml( root, name, prop );

            if ( value == null ) {
               // special case for inline list items
#if !WINDOWS_UWP
               if ( type.IsGenericType )
#else
                    if (type.GetTypeInfo().IsGenericType)
#endif
                    {
                  Type genericType = type.GetGenericArguments()[0];
                  XElement first = this.GetElementByName( root, genericType.Name );
                  IList list = ( IList ) Activator.CreateInstance( type );

                  if ( first != null && root != null ) {
                     IEnumerable<XElement> elements = root.Elements( first.Name );

                     this.PopulateListFromElements( genericType, elements, list );
                  }

                  prop.SetValue( x, list, null );
               }
               continue;
            }

            // check for nullable and extract underlying type
#if !WINDOWS_UWP
            if ( type.IsGenericType && type.GetGenericTypeDefinition() == typeof( Nullable<> ) )
#else
                if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
#endif
                {
               // if the value is empty, set the property to null...
               if ( string.IsNullOrEmpty( value.ToString() ) ) {
                  prop.SetValue( x, null, null );
                  continue;
               }

               type = type.GetGenericArguments()[0];
            }

            if ( type == typeof( bool ) ) {
               string toConvert = value.ToString()
                                       .ToLower();

               prop.SetValue( x, XmlConvert.ToBoolean( toConvert ), null );
            }
#if !WINDOWS_UWP
                else if ( type.IsPrimitive )
#else
                else if (type.GetTypeInfo().IsPrimitive)
#endif
                {
               prop.SetValue( x, value.ChangeType( type, this.Culture ), null );
            }
#if !WINDOWS_UWP
                else if ( type.IsEnum )
#else
                else if (type.GetTypeInfo().IsEnum)
#endif
                {
               object converted = type.FindEnumValue( value.ToString(), this.Culture );

               prop.SetValue( x, converted, null );
            } else if ( type == typeof( Uri ) ) {
               Uri uri = new Uri( value.ToString(), UriKind.RelativeOrAbsolute );

               prop.SetValue( x, uri, null );
            } else if ( type == typeof( string ) ) {
               prop.SetValue( x, value, null );
            } else if ( type == typeof( DateTime ) ) {
               value = this.DateFormat.HasValue()
                   ? DateTime.ParseExact( value.ToString(), this.DateFormat, this.Culture )
                   : DateTime.Parse( value.ToString(), this.Culture );

               prop.SetValue( x, value, null );
            } else if ( type == typeof( DateTimeOffset ) ) {
               string toConvert = value.ToString();

               if ( !string.IsNullOrEmpty( toConvert ) ) {
                  DateTimeOffset deserialisedValue;

                  try {
                     deserialisedValue = XmlConvert.ToDateTimeOffset( toConvert );
                     prop.SetValue( x, deserialisedValue, null );
                  } catch ( Exception ) {
                     object result;

                     if ( TryGetFromString( toConvert, out result, type ) ) {
                        prop.SetValue( x, result, null );
                     } else {
                        //fallback to parse
                        deserialisedValue = DateTimeOffset.Parse( toConvert );
                        prop.SetValue( x, deserialisedValue, null );
                     }
                  }
               }
            } else if ( type == typeof( decimal ) ) {
               value = decimal.Parse( value.ToString(), this.Culture );
               prop.SetValue( x, value, null );
            } else if ( type == typeof( Guid ) ) {
               string raw = value.ToString();

               value = string.IsNullOrEmpty( raw )
                   ? Guid.Empty
                   : new Guid( value.ToString() );

               prop.SetValue( x, value, null );
            } else if ( type == typeof( TimeSpan ) ) {
               TimeSpan timeSpan = XmlConvert.ToTimeSpan( value.ToString() );

               prop.SetValue( x, timeSpan, null );
            }
#if !WINDOWS_UWP
                else if ( type.IsGenericType )
#else
                else if (type.GetTypeInfo(). IsGenericType)
#endif
                {
               Type t = type.GetGenericArguments()[0];
               IList list = ( IList ) Activator.CreateInstance( type );
               XElement container = this.GetElementByName( root, prop.Name.AsNamespaced( this.Namespace ) );

               if ( container.HasElements ) {
                  XElement first = container.Elements().FirstOrDefault();

                  if ( first != null ) {
                     IEnumerable<XElement> elements = container.Elements( first.Name );

                     this.PopulateListFromElements( t, elements, list );
                  }
               }

               prop.SetValue( x, list, null );
            } else if ( type.IsSubclassOfRawGeneric( typeof( List<> ) ) ) {
               // handles classes that derive from List<T>
               // e.g. a collection that also has attributes
               object list = this.HandleListDerivative( root, prop.Name, type );

               prop.SetValue( x, list, null );
            } else {
               //fallback to type converters if possible
               object result;

               if ( TryGetFromString( value.ToString(), out result, type ) ) {
                  prop.SetValue( x, result, null );
               } else {
                  // nested property classes
                  if ( root != null ) {
                     XElement element = this.GetElementByName( root, name );

                     if ( element != null ) {
                        object item = this.CreateAndMap( type, element );

                        prop.SetValue( x, item, null );
                     }
                  }
               }
            }
         }

         return x;
      }

      private static bool TryGetFromString ( string inputString, out object result, Type type ) {

#if !SILVERLIGHT && !WINDOWS_PHONE && !WINDOWS_UWP
         TypeConverter converter = TypeDescriptor.GetConverter( type );

         if ( converter.CanConvertFrom( typeof( string ) ) ) {
            result = ( converter.ConvertFromInvariantString( inputString ) );

            return true;
         }

         result = null;

         return false;
#else
            result = null;

            return false;
#endif
      }

      private void PopulateListFromElements ( Type t, IEnumerable<XElement> elements, IList list ) {
         foreach ( object item in elements.Select( element => this.CreateAndMap( t, element ) ) ) {
            list.Add( item );
         }
      }

      private object HandleListDerivative ( XElement root, string propName, Type type ) {
#if !WINDOWS_UWP
         Type t = type.IsGenericType
             ? type.GetGenericArguments()[0]
             : type.BaseType.GetGenericArguments()[0];
#else
            Type t = type.GetTypeInfo().IsGenericType
               ? type.GetGenericArguments()[0]
               : type.GetTypeInfo().BaseType.GetGenericArguments()[0];
#endif
         IList list = ( IList ) Activator.CreateInstance( type );

         IList<XElement> elements = root.Descendants( t.Name.AsNamespaced( this.Namespace ) ).ToList();

         if ( !elements.Any() ) {
            elements = root.Descendants( "struct" ).ToList();
         }

         string name = t.Name;
         DeserializeAsAttribute attribute = t.GetAttribute<DeserializeAsAttribute>();

         if ( attribute != null ) {
            name = attribute.Name;
         }

         if ( !elements.Any() ) {
            XName lowerName = name.ToLower().AsNamespaced( this.Namespace );

            elements = root.Descendants( lowerName ).ToList();
         }

         if ( !elements.Any() ) {
            XName camelName = name.ToCamelCase( this.Culture ).AsNamespaced( this.Namespace );

            elements = root.Descendants( camelName ).ToList();
         }

         if ( !elements.Any() ) {
            elements = root.Descendants()
                           .Where( e => e.Name.LocalName.RemoveUnderscoresAndDashes() == name )
                           .ToList();
         }

         if ( !elements.Any() ) {
            XName lowerName = name.ToLower().AsNamespaced( this.Namespace );

            elements = root.Descendants()
                           .Where( e => e.Name.LocalName.RemoveUnderscoresAndDashes() == lowerName )
                           .ToList();
         }

         if ( !elements.Any() ) {
            elements = root.Descendants()
                           .ToList();
         }

         this.PopulateListFromElements( t, elements, list );

         // get properties too, not just list items
         // only if this isn't a generic type
#if !WINDOWS_UWP
         if ( !type.IsGenericType )
#else
            if (!type.GetTypeInfo().IsGenericType)
#endif
            {
            this.Map( list, root.Element( propName.AsNamespaced( this.Namespace ) ) ?? root );
            // when using RootElement, the heirarchy is different
         }

         return list;
      }

      protected virtual object CreateAndMap ( Type t, XElement element ) {
         object item;

         if ( t == typeof( string ) ) {
            item = element.Value;
         }
#if !WINDOWS_UWP
            else if ( t.IsPrimitive )
#else
            else if (t.GetTypeInfo().IsPrimitive)
#endif
            {
            item = element.Value.ChangeType( t, this.Culture );
         } else if ( t == typeof( byte[] ) ) {
            item = Convert.FromBase64String( element.Value );
         } else if ( t == typeof( object )) {
            item = element.Value;
         } else {
            item = Activator.CreateInstance( t );
            this.Map( item, element );
         }

         return item;
      }

      protected virtual object GetValueFromXml ( XElement root, XName name, PropertyInfo prop ) {
         object val = null;

         if ( root != null ) {
            XElement element = this.GetElementByName( root, name );

            if ( element == null ) {
               XAttribute attribute = this.GetAttributeByName( root, name );

               if ( attribute != null ) {
                  val = attribute.Value;
               }
            } else {
               if ( !element.IsEmpty || element.HasElements || element.HasAttributes ) {
                  val = element.Value;
               }
            }
         }

         return val;
      }

      protected virtual XElement GetElementByName ( XElement root, XName name ) {
         XName lowerName = name.LocalName.ToLower().AsNamespaced( name.NamespaceName );
         XName camelName = name.LocalName.ToCamelCase( this.Culture ).AsNamespaced( name.NamespaceName );

         if ( root.Element( name ) != null ) {
            return root.Element( name );
         }

         if ( root.Element( lowerName ) != null ) {
            return root.Element( lowerName );
         }

         if ( root.Element( camelName ) != null ) {
            return root.Element( camelName );
         }

         if ( name == "Value".AsNamespaced( name.NamespaceName ) ) {
            return root;
         }

         // try looking for element that matches sanitized property name (Order by depth)
         return root.Descendants()
                    .OrderBy( d => d.Ancestors().Count() )
                    .FirstOrDefault( d => d.Name.LocalName.RemoveUnderscoresAndDashes() == name.LocalName ) ??
                root.Descendants()
                    .OrderBy( d => d.Ancestors().Count() )
                    .FirstOrDefault( d => d.Name.LocalName.RemoveUnderscoresAndDashes() == name.LocalName.ToLower() );
      }

      protected virtual XAttribute GetAttributeByName ( XElement root, XName name ) {
         List<XName> names = new List<XName>
                             {
                                    name.LocalName,
                                    name.LocalName.ToLower()
                                        .AsNamespaced(name.NamespaceName),
                                    name.LocalName.ToCamelCase(this.Culture)
                                        .AsNamespaced(name.NamespaceName)
                                };

         return root.DescendantsAndSelf()
                    .OrderBy( d => d.Ancestors().Count() )
                    .Attributes()
                    .FirstOrDefault( d => names.Contains( d.Name.LocalName.RemoveUnderscoresAndDashes() ) );
      }

      private static XElement TransformXml ( XElement data ) {
         var valueElement = data.Element( "params" ).Element( "param" ).Element( "value" );
         return TransformValue( valueElement, "Response" );
      }

      private static XElement TransformValue ( XElement value, string name ) {

         var child = value.Descendants().First();
         var childName = child.Name;

         if ( childName == "string" || childName == "i4" || childName == "i8" || childName == "int" || childName == "boolean" ||
             childName == "string" || childName == "double" || childName == "dateTime.iso8601" || childName == "base64" ) {
            return new XElement( name, child.Value );
         } else if ( childName == "array" ) {
            return new XElement( name, ExtractArray( child.Element( "data" ).Elements( "value" ), name == "Response" ? null : name ) );
         } else if ( childName == "struct" ) {
            return new XElement( name, ExtractStruct( child.Elements( "member" ) ) );
         }
         return null;
      }

      private static List<XElement> ExtractArray ( IEnumerable<XElement> values, string name ) {
         return values.Select( p => TransformValue( p, name ?? p.Descendants().First().Name.ToString() ) ).ToList();
      }

      private static List<XElement> ExtractStruct ( IEnumerable<XElement> members ) {
         return members.Select( p => ExtractMember( p ) ).ToList();
      }

      private static XElement ExtractMember ( XElement member ) {
         return TransformValue( member.Element( "value" ), ( string ) member.Element( "name" ) );
      }

      //private static bool TryListOfWhatType ( Type type, out Type innerType ) {

      //   var interfaceTest = new Func<Type, Type>( i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof( IList<> ) ? i.GetGenericArguments().Single() : null );

      //   innerType = interfaceTest( type );
      //   if ( innerType != null ) {
      //      return true;
      //   }

      //   foreach ( var i in type.GetInterfaces() ) {
      //      innerType = interfaceTest( i );
      //      if ( innerType != null ) {
      //         return true;
      //      }
      //   }

      //   return false;
      //}
   }
}