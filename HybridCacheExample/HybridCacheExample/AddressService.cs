
using HybridCacheExample.Models;
using Microsoft.Extensions.Caching.Hybrid;
using ZiggyCreatures.Caching.Fusion;

namespace HybridCacheExample
{
    public class AddressService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HybridCache _hybridCache;

        public AddressService(IHttpClientFactory httpClientFactory, HybridCache hybridCache)
        {
            this._httpClientFactory = httpClientFactory;
            _hybridCache = hybridCache;
        }

        public async Task<AddressResponse?> GetCurrentCEP(string cep)
        {
            return await GetCEPAsync(cep);
        }    
        
        public async Task<AddressResponse?> GetCurrentCEPCached(string cep)
        {
            var cacheKey = $"cep:{cep}";

            //Consulta primeiro se já existe o dado no cache antes de buscar na api
            return await _hybridCache.GetOrCreateAsync<AddressResponse?>(cacheKey, async _ => await GetCEPAsync(cep));
        }

        private async Task<AddressResponse?> GetCEPAsync(string cep)
        {
            try
            {
                if (!string.IsNullOrEmpty(cep))
                {
                    var url = $"https://viacep.com.br/ws/{cep}/json/";

                    var client = _httpClientFactory.CreateClient();
                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadFromJsonAsync<AddressResponse>();
                    }
                }
            }
            catch { }

            return null;
        }
    }
}
