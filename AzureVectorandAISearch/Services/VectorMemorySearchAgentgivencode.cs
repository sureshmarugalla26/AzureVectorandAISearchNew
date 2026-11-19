    //using AzureVectorandAISearch.Interfaces;
    //using AzureVectorandAISearch.Models;
    //using OpenAI;
    //using System;
    //using System.Collections.Concurrent;
    //using System.Collections.Generic;
    //using System.Linq;
    //using System.Threading.Tasks;

    //namespace AzureVectorandAISearch.Services
    //{
    //    public class VectorMemorySearchAgentgivencode : IVectorMemoryService
    //    {
    //        // Simple in-memory implementation to avoid Azure SDK dependencies and make the
    //        // project compile and run without Azure configuration. This stores documents
    //        // and performs a basic substring search.

    //        private readonly ConcurrentDictionary<string, DocumentInput> _store = new();

    //        public VectorMemorySearchAgentgivencode(IConfiguration configuration)
    //        {
    //            // configuration is accepted for DI compatibility but not required by this implementation
    //        }

    //        public Task AddDocumentAsync(DocumentInput doc)
    //        {
    //            if (doc == null) throw new ArgumentNullException(nameof(doc));
    //            _store[doc.Id] = doc;
    //            return Task.CompletedTask;
    //        }

    //        public Task<List<string>> SearchAsync(SearchRequest request)
    //        {
    //            if (request == null) throw new ArgumentNullException(nameof(request));

    //            var query = request.Query ?? string.Empty;
    //            var topK = Math.Max(1, request.TopK);

    //            var results = _store.Values
    //                .Where(d => d.Content != null && d.Content.Contains(query, StringComparison.OrdinalIgnoreCase))
    //                .Select(d => d.Content)
    //                .Take(topK)
    //                .ToList();

    //            // If nothing matched, return up to topK documents (fallback)
    //            if (results.Count == 0)
    //            {
    //                results = _store.Values
    //                    .Select(d => d.Content)
    //                    .Take(topK)
    //                    .ToList();
    //            }

    //            return Task.FromResult(results);
    //        }
    //    }
    //}




