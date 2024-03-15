using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.IO.Stores;
using WebRequest = osu.Framework.IO.Network.WebRequest;

namespace EndangerEd.Game.Stores;

/// <summary>
/// Copy of OnlineStore that allows HTTP requests.
/// </summary>
public class AllowHttpOnlineStore : IResourceStore<byte[]>
{
    public async Task<byte[]> GetAsync(string url, CancellationToken cancellationToken = default)
    {
        try
        {
            using (WebRequest req = new WebRequest($@"{url}"))
            {
                await req.PerformAsync(cancellationToken).ConfigureAwait(false);
                return req.GetResponseData();
            }
        }
        catch
        {
            return null;
        }
    }

    public virtual byte[] Get(string url)
    {
        try
        {
            using (WebRequest req = new WebRequest($@"{url}")
                   {
                       AllowInsecureRequests = true
                   })
            {
                req.Perform();
                return req.GetResponseData();
            }
        }
        catch
        {
            return null;
        }
    }

    public Stream GetStream(string url)
    {
        byte[] ret = Get(url);

        if (ret == null) return null;

        return new MemoryStream(ret);
    }

    public IEnumerable<string> GetAvailableResources() => Enumerable.Empty<string>();

    #region IDisposable Support

    public void Dispose()
    {
    }

    #endregion
}
