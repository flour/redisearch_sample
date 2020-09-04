using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autocomplete.Api.Models;
using Autocomplete.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Autocomplete.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ISearcher<ProductModel> _searcher;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            ISearcher<ProductModel> searcher,
            ILogger<ProductsController> logger)
        {
            _searcher = searcher;
            _logger = logger;
        }

        [HttpGet("search/{term}/{limit?}")]
        public async Task<IActionResult> Search(string term, int? limit = 20)
        {
            if (term?.Length < 3)
                return BadRequest("Search term should contain at least 3 chars");

            var result = await _searcher.Search(term, limit.HasValue && limit.Value > 0 ? limit.Value : 20);
            return Ok(result);
        }

        [HttpGet("{productId}")]
        public IActionResult GetProductById(int productId)
        {
            // Stub
            return Ok(new ProductModel { Id = productId });
        }

        [HttpPost("addmultiple")]
        public async Task<IActionResult> AddProduct([FromBody] IEnumerable<ProductModel> models)
        {
            await _searcher.AddDocuments(models);
            return StatusCode(201, new { count = models.Count() });
        }

        [HttpPost("addone")]
        public async Task<IActionResult> AddProduct([FromBody] ProductModel model)
        {
            await _searcher.AddDocument(model);
            return Created(nameof(ProductsController.GetProductById), model.Id);
        }
    }
}