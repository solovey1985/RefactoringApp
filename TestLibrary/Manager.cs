using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{
    public class Manager : IManager
    {
        public string Name { get; set; }
        public bool? IsEnabled { get; set; }
        public Manager ( ) { }

        public Manager ( string name )
        {
            Name = name;
        }
        public string GetName ( string id )
        {
            return $"Name {id}";
        }
        
        public async Task<string> GetNameAsync ( string id )
        {
            return await Task.FromResult($"Name {id}");
        }

        public Task<List<string>> GetNamesByIdAsync ( string id, bool isEmpty )
        {
            if (isEmpty)
                return Task.FromResult(new List<string>());
            else return Task.FromResult(new List<string>() { $"Name {id}", $"Name {id + 1}", $"Name {+2}" });
        }
    }

    public class MyManager : Manager
    {
        public string GetMyNames (string id)
        {
            return base.GetName(id);
        }
    }
}
