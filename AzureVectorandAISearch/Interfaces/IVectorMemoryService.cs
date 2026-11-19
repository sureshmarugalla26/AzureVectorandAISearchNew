using AzureVectorandAISearch.Models;

namespace AzureVectorandAISearch.Interfaces
{
    public interface IVectorMemoryService
    {
        Task AddDocumentAsync(DocumentInput doc);
        Task<List<string>> SearchAsync(SearchRequest request);
    }
}
