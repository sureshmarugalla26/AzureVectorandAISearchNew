namespace AzureVectorandAISearch.Models
{
    public class DocumentInput
    {
        public string Id { get; set; }
        public string Content { get; set; }

        public DocumentInput(string id, string content)
        {
            Id = id;
            Content = content;
        }
    }
}
