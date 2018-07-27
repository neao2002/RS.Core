using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Linq;


namespace RS
{
    public static class AppServiceRegistry
    {
        public static void RegisterAppServices(this IRSServiceCollection services)
        {
            List<Type> implementationTypes = AppServiceTypeFinder.FindFromCompileLibraries();
            RegisterAppServices(services, implementationTypes);
        }

        public static void RegisterAppServices(this IRSServiceCollection services, Assembly assembly)
        {
            List<Type> implementationTypes = AppServiceTypeFinder.Find(assembly);
            RegisterAppServices(services, implementationTypes);
        }

        public static void RegisterAppServicesFromDirectory(this IRSServiceCollection services, string path)
        {
            List<Type> implementationTypes = AppServiceTypeFinder.FindFromDirectory(path);
            RegisterAppServices(services, implementationTypes);
        }

        static void RegisterAppServices(this IRSServiceCollection services, List<Type> implementationTypes)
        {
            Type appServiceType = typeof(IAppService);
            foreach (Type implementationType in implementationTypes)
            {
                var implementedAppServiceTypes = implementationType.GetTypeInfo().ImplementedInterfaces.Where(a => a != appServiceType && appServiceType.IsAssignableFrom(a));

                foreach (Type implementedAppServiceType in implementedAppServiceTypes)
                {
                    if (typeof(AppServiceBase).IsAssignableFrom(implementationType)|| typeof(AppServiceBase<>).IsAssignableFrom(implementationType))
                        services.AddScoped(implementedAppServiceType, implementationType);                    
                    else
                        services.AddTransient(implementedAppServiceType, implementationType);
                }
            }





        }
    }

    internal static class AppServiceTypeFinder
    {
        public static List<Type> FindFromDirectory(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                return new List<Type>();
            }

            var folder = new DirectoryInfo(path);
            FileSystemInfo[] dlls = folder.GetFileSystemInfos("*.dll", SearchOption.TopDirectoryOnly);

            List<Type> ret = new List<Type>();

            foreach (var dll in dlls)
            {
                string lowerName = dll.Name.ToLower();
                if (lowerName.StartsWith("system") || lowerName.StartsWith("microsoft"))
                    continue;

                Assembly assembly;
                try
                {
                    assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(dll.FullName);
                }
                catch (FileLoadException)
                {
                    assembly = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(dll.Name)));

                    if (assembly == null)
                    {
                        throw;
                    }
                }

                ret.AddRange(Find(assembly));
            }

            return ret;
        }
        public static List<Type> FindFromCompileLibraries()
        {
            List<Type> ret = new List<Type>();
            List<Assembly> compileAssemblies = Global.LoadCompileAssemblies();
            foreach (var assembly in compileAssemblies)
            {
                ret.AddRange(Find(assembly));
            }

            return ret;
        }
        public static List<Type> Find(Assembly assembly)
        {
            IEnumerable<Type> allTypes = assembly.GetTypes();

            allTypes = allTypes.Where(a =>
            {
                var b = a.IsAbstract == false && a.IsClass && typeof(IAppService).IsAssignableFrom(a);
                return b;
            });

            List<Type> ret = allTypes.ToList();
            return ret;
        }
        public static List<Type> Find(List<Assembly> assemblies)
        {
            List<Type> ret = new List<Type>();

            foreach (var assembly in assemblies)
            {
                ret.AddRange(Find(assembly));
            }

            return ret;
        }
    }

    //internal static class AssemblyHelper
    //{
    //    public static List<Assembly> LoadCompileAssemblies()
    //    {
    //        List<CompilationLibrary> libs = DependencyContext.Default.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type != "package").ToList();

    //        List<Assembly> ret = new List<Assembly>();
    //        foreach (var lib in libs)
    //        {
    //            Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
    //            ret.Add(assembly);
    //        }

    //        return ret;
    //    }
    //}
}
