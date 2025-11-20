using System.Net.Http.Headers;
using System.Text;

using Newtonsoft.Json;

namespace eology.SharedLibs.ExternalApis.SERanking
{
    public class SERankingSerpService
    {
        private static async Task<List<SER_SerpQueueResultItem>?> SendSERPRequestAsync(SER_SerpApiRequestItem request, string apiKey)
        {
            string jsonPayload = JsonConvert.SerializeObject(request);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Token " + apiKey);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("https://api4.seranking.com/parsing/serp/tasks", content);
            List<SER_SerpQueueResultItem>? queuedRequestlist = new();
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseContent))
                    queuedRequestlist = JsonConvert.DeserializeObject<List<SER_SerpQueueResultItem>>(responseContent);
            }
            else
            {
                Console.WriteLine("Fehler: " + response.StatusCode);
            }
            return queuedRequestlist;
        }

        private static async Task<SER_SERPResult?> GetSERPResultItemAsync(int itemId, string apiKey)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Token " + apiKey);

            HttpResponseMessage response = await client.GetAsync("https://api4.seranking.com/parsing/serp/tasks/" + itemId);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                SER_SERPResult serpResult = JsonConvert.DeserializeObject<SER_SERPResult>(responseContent) ?? new();
                serpResult.results = serpResult.results.Take(10).ToList();
                return serpResult;
            }
            else
            {
                Console.WriteLine("Fehler: " + response.StatusCode);
                return null;
            }
        }

        public static async Task<List<SER_SerpQueueResultItem>?> RequestSerpsFromSERanking(List<string> keywordList, string apiKey)
        {
            SER_SerpApiRequestItem request = new()
            {
                engine_id = 242, // Beispiel-Engine-ID
                query = keywordList // Beispiel-Suchanfrage
            };

            return await SendSERPRequestAsync(request, apiKey);
        }

        public static async Task<SER_SERPResult?> GetTaskItemSerpResult(int itemId, string apiKey)
        {
            return await GetSERPResultItemAsync(itemId, apiKey);
        }

        public static async Task<SER_AccountBalance?> GetAccountBalance(string apiKey)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Token " + apiKey);
            HttpResponseMessage response = await client.GetAsync("https://api4.seranking.com/account/balance");
            string responseContent = await response.Content.ReadAsStringAsync();
            SER_AccountBalance? balanceResult = JsonConvert.DeserializeObject<SER_AccountBalance>(responseContent);
            return balanceResult;
        }
    }
}
