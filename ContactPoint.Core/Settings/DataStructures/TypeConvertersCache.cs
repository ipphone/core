using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using ContactPoint.Common;

namespace ContactPoint.Core.Settings.DataStructures
{
    static class TypeConvertersCache
    {
        private static readonly ConcurrentDictionary<Type, TypeConverter> TypeConverters = new ConcurrentDictionary<Type, TypeConverter>();
        
        public static bool TryGetTypeConverter(this Type type, out TypeConverter typeConverter)
        {
            return (typeConverter = TypeConverters.GetOrAdd(typeof(string), GetTypeConverter)) != null;
        }

        private static TypeConverter GetTypeConverter(Type type)
        {
            return type.GetCustomAttributes(typeof(TypeConverterAttribute), true)
                .Select(x => x as TypeConverterAttribute)
                .Where(x => x != null && !Equals(x, TypeConverterAttribute.Default) && !string.IsNullOrEmpty(x.ConverterTypeName))
                .Join(type.GetCustomAttributesData(), x => x.GetType(), x => x.AttributeType, (x, y) => new {Attribute = x, Data = y})
                .Select(x => (TypeConverterAttribute) x.Data.Constructor.Invoke(x.Data.ConstructorArguments.Select(p => p.Value).ToArray()))
                .Where(x => x != null)
                .Select(x => TypeHelpers.GetTypeByName(x.ConverterTypeName))
                .Where(x => x != null)
                .Select(CreateTypeConverter)
                .FirstOrDefault();
        }

        private static TypeConverter CreateTypeConverter(Type type)
        {
            try
            {
                return (TypeConverter) Activator.CreateInstance(type);
            }
            catch (Exception e)
            {
                Logger.LogWarn(e, $"Can't create instance of '{type.FullName}'");
                return null;
            }
        }
    }
}