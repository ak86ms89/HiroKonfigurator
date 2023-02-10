using HiroKonfig.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HiroKonfig.Services
{
    public interface IAktionService
    {
        Task<IEnumerable<Aktion>> GetByTypName(int type, string forname);
        Task<string> GetByTypNameJson(int type, string forname);   }

    public class AktionService : IAktionService
    {
        private IHttpService _httpService;

        public AktionService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<IEnumerable<Aktion>> GetByTypName(int type, string forname)
        {
            return await _httpService.Post<IEnumerable<Aktion>>("/api/ob/ActivityList/GetOpenByTypName/", new { type, forname });
        }
        public async Task<string> GetByTypNameJson(int type, string forname)
        {
            return await _httpService.PostJson("/api/ob/ActivityList/GetOpenByTypName/", new { type, forname });
        }
    }
}