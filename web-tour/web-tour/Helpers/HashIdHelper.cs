using HashidsNet;
using Microsoft.Extensions.Configuration;

namespace web_tour.Helpers
{
    public class HashIdHelper
    {
        private readonly Hashids _hashids;

        public HashIdHelper(IConfiguration configuration)
        {
            var salt = configuration["HashIdSettings:Salt"] ?? "default_salt";
            var minLength = int.TryParse(configuration["HashIdSettings:MinLength"], out var length) ? length : 12;
            _hashids = new Hashids(salt, minLength);
        }

        public string EncodeId(int id) => _hashids.Encode(id);

        public int DecodeId(string encoded)
        {
            var result = _hashids.Decode(encoded);
            return result.Length > 0 ? result[0] : 0;
        }
    }
}