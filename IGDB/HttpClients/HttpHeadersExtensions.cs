using System.Linq;
using System.Net.Http.Headers;

namespace IGDB.HttpClients
{
    public static class HttpHeadersExtensions
    {
        public static long GetXCountHeader(this HttpHeaders headers)
        {
            long count = 0;
            if (headers.TryGetValues("X-Count", out var values))
            {
                var value = values.FirstOrDefault();
                if (value != null)
                {
                    if (long.TryParse(value, out var countValue))
                    {
                        count = countValue;
                    }
                }
            }

            return count;
        }
    }
}
