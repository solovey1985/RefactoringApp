using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    internal class Program
    {
       static StringBuilder sb = new StringBuilder();

        static void Main ( string[] args )
        {
            
            Console.WriteLine("Provide DLL name");
            string dllName = Console.ReadLine();
            Console.WriteLine("Provide namespace");
            string ns = Console.ReadLine();
            if (string.IsNullOrEmpty(dllName))
            {
                Console.WriteLine($"Using default dll: Kantar.KAP.MS.QF.BL.dll");
                dllName = "Kantar.KAP.MS.QF.BL.dll";
            }

            if (string.IsNullOrEmpty(ns))
            {
                Console.WriteLine($"Using default namespace: Kantar.KAP.MS.QF.Client.APIObjects.DBObjects");
                ns = "Kantar.KAP.MS.QF.BL.Proxies";
            }
            string assemblyDirectory = @"D:\Repository\QF\Site\bin\";
            
            var lib = Assembly.UnsafeLoadFrom(Path.Combine(assemblyDirectory, dllName));
            try
            {
                foreach (Type tt in lib.ExportedTypes.Where(x=>x.Namespace== ns ))
                {
                    if (!tt.IsInterface && tt.BaseType.Name != "Object")
                    {
                        sb.AppendLine($"{tt.Name}:{tt.BaseType.Name}");
                    }
                    else
                    {
                        sb.AppendLine($"{tt.Name}");
                    }
                    foreach (var p in tt.GetProperties())
                    {
                        if (p.PropertyType.IsGenericType)
                        {
                            var generic = p.PropertyType.GetGenericTypeDefinition();
                            string genName = generic.Name;
                            if(generic.Name == "List`1")
                            {
                                genName = "List";
                            }
                            if(generic == typeof(Nullable<>))
                            { 
                                genName= "Nullable";
                            }
                            var pt = p.PropertyType.GetGenericArguments()[0];
                            sb.AppendLine($"\t+{p.Name}:{genName}<{pt.Name}>");
                        }
                        else
                        {
                            sb.AppendLine($"\tt+{p.Name}:{p.PropertyType.Name}");
                        }
                    }
                    GetMethods(tt);
                    sb.AppendLine();
                }

                string fileName = Path.Combine("D:\\Work\\QF", $"{ns}.txt");
                File.WriteAllText(fileName,sb.ToString());
                Console.WriteLine($"File {fileName} was created");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                string fileName = Path.Combine("D:\\Work\\QF", $"{ns}.txt");
                File.WriteAllText(fileName, sb.ToString());
            }
            Console.ReadLine();
        }

        static void GetMethods (Type typeInfo)
        {
            var methods = typeInfo.GetMethods().Where(x=>x.DeclaringType.Name == typeInfo.Name && x.IsPublic);
            if (methods.Any())
            {
                sb.AppendLine("--");
                foreach (var m in methods)
                {
                    if (m.Name.StartsWith("get_") || m.Name.StartsWith("set_")) continue;
                    var returnType = GetReturnType(m);
                    sb.AppendLine($"{m.Name}():{returnType}");
                }
            }
        }

        static string GetReturnType (MethodInfo method)
        {
            if (method.ReturnType.IsGenericType)
            {
                var genericType = method.ReturnType.Name.Replace("`1","");
                if(genericType == "Task")
                {
                   var taskType = method.ReturnType.GetGenericArguments()[0];
                    var entype = taskType.GetGenericArguments()[0];
                    if(entype.IsGenericType && entype.GetGenericArguments().Length>0)
                    {
                        return $"{entype.Name.Replace("`1", "")}<{entype.GetGenericArguments()[0].Name}>";
                    }
                    return $"Task<{entype.Name}>";
                }
                var enclosedType = method.ReturnType.GetGenericArguments()[0];
                Console.WriteLine($" {method.Name}: {genericType}<{enclosedType.Name.Replace("`1", "")}>");
                return $"{genericType}<{enclosedType.Name.Replace("`1", "")}>";
            }
            else
            {
                return method.ReturnType.Name;
            }
        }
    }
}
