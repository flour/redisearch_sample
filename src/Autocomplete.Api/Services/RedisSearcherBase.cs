using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autocomplete.Api.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NRediSearch;
using StackExchange.Redis;

namespace Autocomplete.Api.Services
{
    public abstract class RedisSearcherBase<T> : ISearcher<T> where T : IdModel
    {
        protected readonly ILogger _logger;
        protected ConnectionMultiplexer Redis { get; }
        protected IDatabase Database { get; }
        protected Client Client { get; private set; }

        public RedisSearcherBase(
            string clientName, 
            IOptions<RedisSettings> redisOptions, 
            ILogger logger)
        {
            _logger = logger;
            var configuration = new ConfigurationOptions();
            foreach (var hostAddress in redisOptions.Value.Hosts.Split(","))
                configuration.EndPoints.Add(hostAddress);
            configuration.Password = redisOptions.Value.Password;
            configuration.ClientName = redisOptions.Value.ClientName;

            Redis = ConnectionMultiplexer.Connect(configuration);
            Database = Redis.GetDatabase();
            Client = new Client(clientName, Database);

            EnsureIndexCreated(MapObjectToSchema(typeof(T))).GetAwaiter().GetResult();
        }

        public abstract Task<IEnumerable<T>> Search(string term, int limit);

        public virtual async Task AddDocument(T document)
        {
            await Client.AddDocumentAsync(document.Id.ToString(), MapToRedis(document), 1);
        }

        public virtual async Task AddDocuments(IEnumerable<T> documents)
        {
            if (documents.Any())
                await Client.AddDocumentsAsync(
                    documents.Select(d => new Document(d.Id.ToString(), MapToRedis(d))).ToArray());
        }

        internal virtual async Task<bool> EnsureIndexCreated(Schema schema)
        {
            try
            {
                return await Client.CreateIndexAsync(schema, new Client.ConfiguredIndexOptions(Client.IndexOptions.Default));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "qwe");
                return true;
            }
        }

        internal Schema MapObjectToSchema()
            => MapObjectToSchema(typeof(T));

        internal Schema MapObjectToSchema(Type t)
        {
            var schema = new Schema();
            var typeProperies = t.GetProperties()
                .Select(prop => new { prop.Name, Code = Type.GetTypeCode(prop.PropertyType) });
            foreach (var prop in typeProperies)
            {
                switch (prop.Code)
                {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Single:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                        schema.AddNumericField(prop.Name);
                        break;
                    default:
                        schema.AddTextField(prop.Name.ToLower());
                        break;
                }
            }
            return schema;
        }

        internal Dictionary<string, RedisValue> MapToRedis(T value)
        {
            return typeof(T).GetProperties()
                .Select(prop => new { prop.Name, Value = prop.GetValue(value) })
                .ToDictionary(x => x.Name.ToLower(), x => new RedisValue(x.Value?.ToString()));
        }

        internal IEnumerable<T> CastRedisValues(List<Document> documents)
        {
            foreach (var doc in documents)
            {
                var jsonRedisItem = JsonConvert.SerializeObject(doc.GetProperties());
                yield return JObject.Parse(jsonRedisItem).ToObject<T>();
            }
        }
    }
}