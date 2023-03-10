using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IGDB.HttpClients
{
    public class XCountHeaderHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            long totalCount = response.Headers.GetXCountHeader();

            response.Content.Headers.Add("X-Count", totalCount.ToString());
            return response;
        }
    }
}
