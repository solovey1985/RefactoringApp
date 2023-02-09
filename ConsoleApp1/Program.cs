// See https://aka.ms/new-console-template for more information
using System.Reflection;
string assemblyDirectory = @"D:\Solutions\BlackOutApp\ConsoleApp1\ConsoleApp1\bin\Debug\net6.0\";
var lib = Assembly.ReflectionOnlyLoad(Path.Combine(assemblyDirectory,"Kantar.KAP.MS.QF.BL.dll"));
try {
    foreach (Type t in lib.GetTypes())
    {
        Console.WriteLine(t.FullName);
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Console.ReadLine();
