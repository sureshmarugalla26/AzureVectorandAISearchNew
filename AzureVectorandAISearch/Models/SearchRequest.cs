namespace AzureVectorandAISearch.Models
{
    public class SearchRequest
    {
        public string Query { get; set; }
        public int TopK { get; set; }

        public SearchRequest(string query, int topK = 3)
        {
            Query = query;
            TopK = topK;
        }
    }
}
