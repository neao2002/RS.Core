using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Linq;


namespace RS
{
    /// <summary>
    /// 
    /// </summary>
    public static class AppServiceRegistry
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void RegisterAppServices(this IRSServiceCollection services)
        {
            List<Type> implementationTypes = AppServiceTypeFinder.FindFromCompileLibraries();
            RegisterAppServices(services, implementationTypes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        public static void RegisterAppServices(this IRSServiceCollection services, Assembly assembly)
        {
            List<Type> implementationTypes = AppServiceTypeFinder.Find(assembly);
            RegisterAppServices(services, implementationTypes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="path"></param>
        public static void RegisterAppServicesFromDirectory(this IRSServiceCollection services, string path)
        {
            List<Type> implementationTypes = AppServiceTypeFinder.FindFromDirectory(path);
            RegisterAppServices(services, implementationTypes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="implementationTypes"></param>
        static void RegisterAppServices(this IRSServiceCollection services, List<Type> implementationTypes)
        {
            //需要自动注册的服务类型
            List<Type> ServiceTypes = new List<Type>(){
                typeof(IAppService)
            };
            foreach (Type implementationType in implementationTypes)
            {
                var implementedAppServiceTypes = implementationType.GetTypeInfo().ImplementedInterfaces.Where(a => IsAssignableFrom(ServiceTypes,a));

                foreach (Type implementedAppServiceType in implementedAppServiceTypes)
                {
                    //如果是会话服务对象，则进行单例创建,也就是每一类角色，只会创建一个提供者角色
                    if (typeof(AppServiceBase).IsAssignableFrom(implementationType) || typeof(AppServiceBase<>).IsAssignableFrom(implementationType))
                        services.AddScoped(implementedAppServiceType, implementationType);
                    else //其它的则每次要用时都创建一个实例
                        services.AddTransient(implementedAppServiceType, implementationType);
                }
            }

        }

        static bool IsAssignableFrom(List<Type> types,Type a)
        {
            return types.Exists(appServiceType => a != appServiceType && appServiceType.IsAssignableFrom(a));
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
