using Azure;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using AzureVectorandAISearch.Interfaces;
using AzureVectorandAISearch.Models;
using OpenAI.Embeddings;

namespace AzureVectorandAISearch.Services
{
    public class VectorMemoryService : IVectorMemoryService
    {
        private readonly AzureOpenAIClient _azureClient;
        private readonly EmbeddingClient _embeddingClient;
        private readonly SearchClient _searchClient;
        private readonly SearchIndexClient _indexClient;
        private readonly string _indexName;

        public VectorMemoryService(IConfiguration configuration)
        {
            var openAiEndpoint = configuration["AzureOpenAI:EndPoint"]!;
            var openAiKey = configuration["AzureOpenAI:APIKey"]!;
            var embeddingDeployment = configuration["AzureOpenAI:EmbeddingDeployment"]!;

            var searchEndpoint = configuration["AzureSearch:EndPoint"]!;
            var searchKey = configuration["AzureSearch:APIKey"]!;
            _indexName = configuration["AzureSearch:IndexName"] ?? "documents";

            // ✅ Azure OpenAI client
            _azureClient = new AzureOpenAIClient(
                new Uri(openAiEndpoint),
                new AzureKeyCredential(openAiKey));

            _embeddingClient = _azureClient.GetEmbeddingClient(embeddingDeployment);

            // ✅ Azure Search clients
            _searchClient = new SearchClient(
                new Uri(searchEndpoint),
                _indexName,
                new AzureKeyCredential(searchKey));

            _indexClient = new SearchIndexClient(
                new Uri(searchEndpoint),
                new AzureKeyCredential(searchKey));

            CreateIndexIfNotExistsAsync().GetAwaiter().GetResult();
        }

       

        // ========================================
        // Add Document  first time This text is converted to numbers   
        //👉 Those numbers are stored in Azure Search
        // ========================================
        public async Task AddDocumentAsync(DocumentInput doc)
        {
            var embedding = await GetEmbeddingAsync(doc.Content);

            var document = new
            {
                id = doc.Id,
                content = doc.Content,
                vector = embedding
            };
            //storing or uploading the data to Azure Search
            await _searchClient.MergeOrUploadDocumentsAsync(new[] { document });
        }

        // ========================================
        // Vector Search means searching the numbers stored in Azure Search and comparing them with the numbers generated from the query
        //means second time if we enter or search data then it will convert the query to numbers and then it will search those numbers
        ///in Azure Search and return the most relevant results based on the similarity of the vectors
        // ========================================
        public async Task<List<string>> SearchAsync(SearchRequest request)
        {
            var queryVector = await GetEmbeddingAsync(request.Query);

            var vectorQuery = new VectorizedQuery(
                new ReadOnlyMemory<float>(queryVector))
            {
                KNearestNeighborsCount = request.TopK,
                Fields = { "vector" }
            };

            var options = new SearchOptions
            {
                Size = request.TopK,
                VectorSearch = new VectorSearchOptions
                {
                    Queries = { vectorQuery }
                }
            };

            var response = await _searchClient.SearchAsync<SearchDocument>("*", options);

            var results = new List<string>();

            await foreach (var result in response.Value.GetResultsAsync())
            {
                results.Add(result.Document["content"]?.ToString() ?? "");
            }

            return results;
        }
        // ========================================
        // Generate Embedding
        // ========================================
        private async Task<float[]> GetEmbeddingAsync(string text)
        {
            var response = await _embeddingClient.GenerateEmbeddingsAsync(new[] { text });

            return response.Value[0].ToFloats().ToArray();
        }

        // ========================================
        // Create Index (Azure Search 11.7.0)
        // ========================================
        private async Task CreateIndexIfNotExistsAsync()
        {
            try
            {
                await _indexClient.GetIndexAsync(_indexName);
            }
            catch (RequestFailedException)
            {
                var fields = new List<SearchField>
                {
                    new SimpleField("id", SearchFieldDataType.String)
                    {
                        IsKey = true
                    },

                    new SearchableField("content"),

                    new SearchField("vector",
                        SearchFieldDataType.Collection(SearchFieldDataType.Single))
                    {
                        IsSearchable = true,
                        VectorSearchDimensions = 1536,
                        VectorSearchProfileName = "vector-profile"
                    }
                };

                var index = new SearchIndex(_indexName, fields)
                {
                    VectorSearch = new VectorSearch
                    {
                        Profiles =
                        {
                            new VectorSearchProfile("vector-profile", "hnsw-config")
                        },
                        Algorithms =
                        {
                            new HnswAlgorithmConfiguration("hnsw-config")
                        }
                    }
                };

                await _indexClient.CreateIndexAsync(index);
            }
        }
    }
}