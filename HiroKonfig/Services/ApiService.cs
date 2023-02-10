namespace HiroKonfig.Services
{
    public interface IApiService
    {
        Task<string> Post(string uri, object value);

    }
    public class ApiService : IApiService
    {
        public ApiService(IHttpService httpService)
        {
            _httpService = httpService;
        }
        private IHttpService _httpService;

        public async Task<string> Post(string uri, object value)
        {
            string jsonstream = await _httpService.Post<string>(uri, value);
            return jsonstream;
        }
    }
}
