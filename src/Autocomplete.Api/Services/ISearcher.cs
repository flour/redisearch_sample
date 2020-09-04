using System.Collections.Generic;
using System.Threading.Tasks;
using Autocomplete.Api.Models;

namespace Autocomplete.Api.Services
{
    public interface ISearcher<T> where T : IdModel
    {
        Task AddDocument(T document);
        Task AddDocuments(IEnumerable<T> documents);
        Task<IEnumerable<T>> Search(string term, int limit);
    }
}