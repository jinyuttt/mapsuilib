using BruTile;
using BruTile.Web;

namespace AvaloniaMapsuiLib
{
   public class HttpClientTileSource:ITileSource,IHttpTileSource,ILocalTileSource
    {
        private readonly HttpClient _HttpClient;
        private readonly HttpTileSource _WrappedSource;

        public HttpClientTileSource(HttpClient httpClient, ITileSchema tileSchema, string urlFormatter, IEnumerable<string> serverNodes = null, string apiKey = null, string name = null, BruTile.Cache.IPersistentCache<byte[]> persistentCache = null)
        {
           // Attribution attribution
            _HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _WrappedSource = new HttpTileSource(tileSchema, urlFormatter, serverNodes, apiKey, name, persistentCache);
        }

        public ITileSchema Schema => _WrappedSource.Schema;

        public string Name => _WrappedSource.Name;

        public Attribution Attribution => _WrappedSource.Attribution;


        public Task<byte[]?> GetTileAsync(TileInfo tileInfo)
        {
            return _WrappedSource.GetTileAsync(_HttpClient,tileInfo);
          
        }

        public Task<byte[]?> GetTileAsync(HttpClient httpClient, TileInfo tileInfo, CancellationToken? cancellation = null)
        {
            return _WrappedSource.GetTileAsync(_HttpClient, tileInfo, cancellation);
        }

        private Task<byte[]?> ClientFetch(Uri uri) => _HttpClient.GetByteArrayAsync(uri);
    }
}
