using System;
using System.Linq;
using System.Reflection;

namespace ConsoleMenuHelper
{
    /// <summary>Extensions used with the IServiceProvider (dependency injection framework).</summary>
    /// <remarks>Code was found here: https://stackoverflow.com/a/40334745/97803 and slightly altered.</remarks>
    public static class ServiceProviderExtensions
    {
        /// <summary>Creates an instance of an object, but obtains its dependencies from the service provider.</summary>
        /// <param name="provider">The service provider that holds all the dependencies</param>
        /// <param name="theType">The type that is being created.</param>
        /// <returns>An instance of the object being created.</returns>
        public static object CreateInstance(this IServiceProvider provider, Type theType)
        {
            ConstructorInfo constructor = theType.GetConstructors()[0];

            if (constructor != null)
            {
                object[] args = constructor
                    .GetParameters()
                    .Select(o => o.ParameterType)
                    .Select(o => provider.GetService(o))
                    .ToArray();

                return Activator.CreateInstance(theType, args);
            }

            return null;
        }

        /// <summary>Creates an instance of an object, but obtains its dependencies from the service provider.</summary>
        /// <param name="provider">The service provider that holds all the dependencies</param>
        /// <returns>An instance of the object being created.</returns>
        /// <remarks>Code was found here: https://stackoverflow.com/a/40334745/97803 (using altered version above).</remarks>
        public static T CreateInstance<T>(this IServiceProvider provider) where T : class
        {
            ConstructorInfo constructor = typeof(T).GetConstructors()[0];

            if(constructor != null)
            {
                object[] args = constructor
                    .GetParameters()
                    .Select(o => o.ParameterType)
                    .Select(o => provider.GetService(o))
                    .ToArray();

                return Activator.CreateInstance(typeof(T), args) as T;
            }

            return null;
        }
    }
}