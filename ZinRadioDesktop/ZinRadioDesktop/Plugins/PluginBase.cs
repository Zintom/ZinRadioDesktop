using System;
using System.IO;
using System.Reflection;

namespace ZinRadioDesktop.Plugins
{
    public class PluginInfo
    {
        public string Name { get; init; }

        public PluginInfo(string name)
        {
            Name = name;
        }
    }

    public static class PluginBase
    {
        public static readonly string PluginDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\Plugins";

        /// <summary>
        /// Creates the <b>Plugins</b> directory if it does not exist already.
        /// </summary>
        public static void EnsurePluginDirectoryExists()
        {
            if (Directory.Exists(PluginDirectory))
            {
                return;
            }

            Directory.CreateDirectory(PluginDirectory);
        }

        /// <summary>
        /// Locates the first <typeparamref name="T"/> in the given <paramref name="dllFile"/> assembly and instantiates it.
        /// </summary>
        /// <param name="dllFile"></param>
        /// <returns>An instantiation of the first located <typeparamref name="T"/>, or <see langword="null"/> if no types implement the interface in the assembly.</returns>
        public static T? CreateTypeFromDll<T>(string dllFile) where T : class
        {
            // Search through all the types in the assembly.
            foreach (Type type in Assembly.LoadFrom(dllFile).GetTypes())
            {
                // If the type matches our target type, try to instantiate it.
                if (type is T)
                {
                    return (T?)Activator.CreateInstance(type);
                }
            }

            return null;
        }
    }
}
