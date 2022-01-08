using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;

namespace Integration.UnitTest
{
    public class BaseTest
    {
        protected const string BaseUrl = "https://localhost:5022/";

        protected async Task<T> PostAsync<T>(string endPoint, object dto)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest(endPoint, Method.Post);
            request.AddHeader("Content-Type", "application/json");
            var body = JsonConvert.SerializeObject(dto);
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            RestResponse response = await client.ExecuteAsync(request);
            return JsonConvert.DeserializeObject<T>(response.Content);
        }
        protected async Task<T> GetAsync<T>(string endPoint, string token)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest(endPoint, Method.Get);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {token}");
            RestResponse response = await client.ExecuteAsync(request);
            return JsonConvert.DeserializeObject<T>(response.Content);
        }
        protected async Task<T> DeleteAsync<T>(string endPoint, string token)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest(endPoint, Method.Delete);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {token}");
            RestResponse response = await client.ExecuteAsync(request);
            return JsonConvert.DeserializeObject<T>(response.Content);
        }
    }
}
