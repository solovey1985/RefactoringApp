using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestLibrary
{
    public interface IManager: IHelper
    {
        Task<string> GetNameAsync ( string id );
        Task<List<string>> GetNamesByIdAsync ( string id, bool isEmpty );
    }

    public interface IHelper
    {

    }
}