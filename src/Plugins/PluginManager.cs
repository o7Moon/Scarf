using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace linerider.Plugins
{
    public class PluginManager
    {
        public PluginManager()
        {
        }
        public static List<Plugin> loadedPlugins;
        public static Dictionary<string, Plugin> pluginsByName;
        public static void Init() {
            loadedPlugins = new List<Plugin>();
            pluginsByName = new Dictionary<string, Plugin>();
            string pluginsPath = Program.UserDirectory+"Plugins";
            string[] pluginPaths = Directory.GetFiles(pluginsPath, "*.dll", SearchOption.AllDirectories);

            foreach (string path in pluginPaths) { 
                try {
                    Assembly pluginAssembly = Assembly.LoadFrom(path);
                } catch (System.Exception e) {
                    Console.WriteLine(e);
                }
            }
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) { 
                foreach (Type t in assembly.GetTypes()) { 
                    if (t.BaseType == typeof(Plugin)) {
                        Plugin plugin = (Plugin)Activator.CreateInstance(t);
                        plugin.Load();
                        plugin.name = t.Namespace+":"+t.Name;
                        pluginsByName.Add(plugin.name,plugin);
                        loadedPlugins.Add(plugin);
                    }
                }
            }
        }
    }
}
