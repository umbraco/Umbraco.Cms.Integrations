using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Helpers
{
    public class FormHelper
    {
        public static IEnumerable<MethodInfo> GetMethodsForType(Type type)
        {
            foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                yield return method;
            }

            if (type.IsInterface)
            {
                foreach (var iface in type.GetInterfaces())
                {
                    foreach (var method in GetMethodsForType(iface))
                    {
                        yield return method;
                    }
                }
            }
        }

        public static MethodInfo GetMethodForTypeByName(Type type, string name, object[] parameters) =>
            GetMethodsForType(type).First(p => p.Name == name && p.GetParameters().Length == parameters.Length);
    }
}
