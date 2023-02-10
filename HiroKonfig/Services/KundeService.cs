using HiroKonfig.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HiroKonfig.Services
{
    public interface IKundeService
    {
        Task<Kunde> GetByID(int id);
        Task<string> GetByIDJson(int id);
    }
    public class KundeService : IKundeService
    {
        private IHttpService _httpService;

        public KundeService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<Kunde> GetByID(int id)
        {
            return await _httpService.Post<Kunde>("/api/ob/Customer/GetByID/", new { id });
        }
        public async Task<string> GetByIDJson(int id)
        {
            return await _httpService.PostJson("/api/ob/Customer/GetByID/", new { id });
        }
    }
}