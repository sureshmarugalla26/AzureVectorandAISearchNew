using AzureVectorandAISearch.Interfaces;
using AzureVectorandAISearch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureVectorandAISearch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VectorMemoryController : ControllerBase
    {
        private readonly IVectorMemoryService _vectorService;

        public VectorMemoryController(IVectorMemoryService vectorService)
        {
            _vectorService = vectorService;
        }

        [HttpPost("adddocument")]
        public async Task<IActionResult> AddDocument([FromBody] DocumentInput doc)
        {
            await _vectorService.AddDocumentAsync(doc);
            return Ok(new { message = "Document added and indexed." });
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
            var results = await _vectorService.SearchAsync(request);
            return Ok(results);
            //return null;
        }
    }
}
