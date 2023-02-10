using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsoleApp2
{
    internal class Program
    {
        static StringBuilder sb = new StringBuilder();
        static string dllName;
        static string dllPath;
        static string ns;
        static void Main ( string[] args )
        {
            var strSettings = File.ReadAllText("settings.json");
            Settings settings = JsonConvert.DeserializeObject<Settings>(strSettings);
            if (settings != null)
            {
                Console.WriteLine("Settings from file:");
                Console.WriteLine("Directory:" + settings.Directory);
                Console.WriteLine("Library:" + settings.Library);
            }
            Console.WriteLine("Provide DLL name");
            dllName = Console.ReadLine();
            Console.WriteLine("Provide namespace");
            string ns = Console.ReadLine();
            if (string.IsNullOrEmpty(dllName))
            {
                Console.WriteLine($"Using default dll: {settings.Library}");
                dllName = settings.Library;
            }

            var lib = Assembly.UnsafeLoadFrom(Path.Combine(settings.Directory, dllName));
            try
            {
                foreach (var n in settings.Namespaces)
                {
                 
                    ns = n.Name;
                    var tts = lib.ExportedTypes.Where(x => x.Namespace.StartsWith(ns)).ToList();
                    Console.WriteLine($"Types found: {tts.Count}");
                    Console.WriteLine("Getting info for " + ns);
                    File.Delete(Path.Combine(settings.OutputDirectory, $"{ns}.txt"));
                    Console.ReadLine();
                    foreach (Type tt in tts)
                    {

                        if (tt.IsEnum)
                        {
                            HandleEnumType(tt);
                            continue;
                        }
                        if (tt.IsInterface)
                        {
                            var interfaces = tt.GetInterfaces();
                            var ii = interfaces.Select(x => x.Name).ToArray();
                            if (ii.Length > 0)
                            {
                                sb.AppendLine($"[{tt.Name}] {ii[0]}");
                            }
                            else
                            {
                                sb.AppendLine($"[{tt.Name}]");
                            }
                        }
                        if (!tt.IsInterface)
                        {
                            string inheritedFrom = "";
                            if (tt.BaseType.Name != "Object")
                            {
                                inheritedFrom = tt.BaseType.Name;
                            }
                            var ii = GetInterfaces(tt);
                            sb.AppendLine($"{tt.Name}:{inheritedFrom} {ii}");
                        }
                        foreach (var p in tt.GetProperties())
                        {
                            if (p.PropertyType.IsGenericType)
                            {
                                var generic = p.PropertyType.GetGenericTypeDefinition();
                                string genName = generic.Name;
                                if (generic.Name == "List`1")
                                {
                                    genName = "List";
                                }
                                if (generic == typeof(Nullable<>))
                                {
                                    genName = "Nullable";
                                }
                                var pt = p.PropertyType.GetGenericArguments()[0];
                                sb.AppendLine($"\t{p.Name}:{genName}<{pt.Name}>");
                            }
                            else
                            {
                                sb.AppendLine($"\t+{p.Name}:{p.PropertyType.Name}");
                            }
                        }
                        GetMethods(tt);
                        sb.AppendLine();
                        File.AppendAllText(Path.Combine(settings.OutputDirectory, $"{ns}.txt"), sb.ToString());
                        sb.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                string fileName = Path.Combine("D:\\Work\\QF", $"{ns}.txt");
                File.WriteAllText(fileName, sb.ToString());
            }
            Console.ReadLine();
        }

        private static string GetInterfaces ( Type tt )
        {
            var interfaces = tt.GetInterfaces();
            var ii = interfaces.Select(x => x.Name).ToArray();
            var isb = new StringBuilder();
            foreach (var i in ii)
            {
                isb.Append($"{i},");
            }
            return sb.ToString().Trim(',');
        }

        private static void HandleEnumType ( Type tt )
        {
            sb.AppendLine($"enum {tt.Name}");
            var vals = tt.GetEnumNames();
            foreach (var val in vals)
            {
                sb.AppendLine(val);
            }
            sb.AppendLine();
        }

        static void GetMethods ( Type typeInfo )
        {
            var methods = typeInfo.GetMethods()
                .Where(x => x.IsPublic && x.DeclaringType == typeInfo
                    && !(x.Name.StartsWith("get_") || x.Name.StartsWith("set_"))
                );
            if (methods.Any())
            {
                sb.AppendLine("--");
                foreach (var m in methods)
                {
                    var returnType = GetReturnType(m);
                    sb.AppendLine($"{m.Name}():{returnType}");
                }
            }
        }

        static string GetReturnType ( MethodInfo method )
        {
            Console.WriteLine(method.Name + " " + method.ReturnType.ToString());

            var arguments = GetArguments(method.ReturnType);
            return arguments;
        }
        static string GetArguments ( Type argument )
        {

            if (argument.IsGenericType && argument.GetGenericArguments().Length > 0)
            {
                string genArument = GetArguments(argument.GetGenericArguments()[0]);
                string argName = argument.Name.Replace("`1", "");
                return $"{argName}<{genArument}>";
            }
            else
            {
                return argument.Name;
            }
        }
    }
}
