using System;
using System.Linq;
using System.Reflection;

namespace ConsoleMenuHelper
{
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

        // https://stackoverflow.com/a/40334745/97803
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static TResult CreateInstance<TResult>(this IServiceProvider provider) where TResult : class
        {
            ConstructorInfo constructor = typeof(TResult).GetConstructors()[0];

            if(constructor != null)
            {
                object[] args = constructor
                    .GetParameters()
                    .Select(o => o.ParameterType)
                    .Select(o => provider.GetService(o))
                    .ToArray();

                return Activator.CreateInstance(typeof(TResult), args) as TResult;
            }

            return null;
        }
    }
}