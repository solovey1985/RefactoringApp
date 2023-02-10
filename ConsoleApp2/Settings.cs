using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    internal class Settings
    {
        public string Directory { get; set; }
        public string Library { get; set; }
        public List<Spaces> Namespaces{ get; set; }
        public string OutputDirectory { get; set; }
    }

    public class Spaces
    {
        public string Name { get; set; }
    }
}
