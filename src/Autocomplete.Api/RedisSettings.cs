using System.Collections.Generic;

namespace Autocomplete.Api
{
    public class RedisSettings
    {
        public List<string> Hosts { get; set; }
        public string Password { get; set; }
        public string ClientName { get; set; }
    }
}