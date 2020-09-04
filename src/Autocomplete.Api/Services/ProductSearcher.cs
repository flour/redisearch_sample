using System.Collections.Generic;
using System.Threading.Tasks;
using Autocomplete.Api.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NRediSearch;

namespace Autocomplete.Api.Services
{
    public class ProductSearcher : RedisSearcherBase<ProductModel>
    {
        public ProductSearcher(
            IOptions<RedisSettings> redisOptions,
            ILogger<ProductSearcher> logger)
            : base(nameof(ProductSearcher), redisOptions, logger)
        {
        }

        public override async Task<IEnumerable<ProductModel>> Search(string term, int limit)
        {
            await EnsureIndexCreated(MapObjectToSchema());
            var query = new Query($"(@{nameof(ProductModel.Name).ToLower()}:{term}*)")
                .Limit(0, limit);
            var result = await Client.SearchAsync(query);
            var documents = result.Documents;
            return CastRedisValues(documents);
        }
    }
}