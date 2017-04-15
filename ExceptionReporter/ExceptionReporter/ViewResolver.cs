using System;
using System.Reflection;

namespace ExceptionReporter
{
	/// <summary>
	/// Resolve a view from an assembly (limited to ExceptionReportView and InternalExceptionView)
	/// This flexibility is required in order to load either a WPF or WinForms version of the view class
	/// </summary>
	public class ViewResolver
	{
		private readonly Assembly _assembly;

		///<summary>
		/// Initialise the ViewResolver with an assembly to search
		///</summary>
		///<param name="assembly">the Assembly which contains the desired view</param>
		public ViewResolver(Assembly assembly)
		{
			_assembly = assembly;
		}

		/// <summary>
		/// Resolve an interface to a concrete view class, limited to 2 particular expected 'View' classes in ExceptionReporter
		/// </summary>
		/// <typeparam name="T">The interface type (currenty just IExceptionReportView or IInternalExceptionView)</typeparam>
		/// <returns>An instance of a type that implements the interface (T) in the given assembly (see constructor)</returns>
		public Type Resolve<T>() where T : class
		{
			var viewType = typeof(T);

            //var assemblies = _assembly.GetReferencedAssemblies();
            //assemblies.Concat(new[]{new AssemblyName (_assembly.FullName)});

            //var matchingTypes =
            //    from assemblyName in
            //        from assembly in assemblies
            //        where assembly.Name.Contains("ExceptionReporter")
            //        select assembly
            //    from type in Assembly.Load(assemblyName.Name).GetExportedTypes()
            //    where !type.IsInterface
            //    where viewType.IsAssignableFrom(type)
            //    select type;

            //if (matchingTypes.Count() == 1)
            //    return matchingTypes.First();
			
            //throw new ApplicationException(string.Format("Unable to resolve single instance of '{0}'", viewType));

            AssemblyName[] assemblies = _assembly.GetReferencedAssemblies();

            if (_assembly.FullName.Contains("ExceptionReporter"))
            {
                foreach (Type type in Assembly.Load(_assembly.FullName).GetExportedTypes())
                {
                    if (!type.IsInterface && viewType.IsAssignableFrom(type))
                        return type;
                }
            }

            foreach (var assembly in assemblies)
            {
                if (assembly.Name.Contains("ExceptionReporter"))
                {
                    foreach (Type type in Assembly.Load(assembly.Name).GetExportedTypes())
                    {
                        if (!type.IsInterface && viewType.IsAssignableFrom(type))
                            return type;
                    }
                }
            }

            throw new ApplicationException(string.Format("Unable to resolve single instance of '{0}'", viewType));
        }
	}
}