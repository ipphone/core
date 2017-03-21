using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace ContactPoint.Core.Helpers
{
    internal static class ReflectionPropertyCloner
    {
        public static T ReflectionClone<T>(this T source)
            where T: class, new()
        {
            T result = new T();
            var type = typeof(T);

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.GetProperty);

            foreach (var property in properties)
            {
                if (property.GetCustomAttributes(typeof(DoNotCloneAttribute), false).Length == 0) continue;

                if (property.CanWrite) // Clone property
                    property.SetValue(result, property.GetValue(source, null), null);
                else { // Clone collection
                    var genericArguments = property.PropertyType.GetGenericArguments();

                    if (genericArguments.Count() == 1)
                    {
                        var targetType = typeof(ICollection).MakeGenericType(genericArguments);

                        var sourceCollection = property.GetValue(source, null);
                        var targetCollection = property.GetValue(result, null);

                        if (sourceCollection != null && targetCollection != null)
                        {
                            foreach (var item in (IEnumerable)sourceCollection)
                            {
                                targetType.InvokeMember("Add", BindingFlags.InvokeMethod | BindingFlags.Public, null, targetCollection, new object[] { item });
                            }
                        }
                    }
                }
            }

            return result;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DoNotCloneAttribute : Attribute
    { }
}
