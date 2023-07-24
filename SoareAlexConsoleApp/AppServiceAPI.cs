using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SoareAlexConsoleApp
{
    public class AppServiceAPI
    {
        private readonly string baseApi;

        public AppServiceAPI(string baseApi)
        {
            this.baseApi = baseApi;
        }

        public async Task PostRequest<T>(string api, Object requestBody, Action<T> responseCallback)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var url = baseApi + api;

                    var json = JsonSerializer.Serialize(requestBody);

                    byte[] bytes = Encoding.ASCII.GetBytes(json);

                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await httpClient.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(responseContent))
                        {
                            var responseObj = JsonSerializer.Deserialize<T>(responseContent);
                            responseCallback(responseObj);
                        }
                        else
                        {
                            Console.WriteLine($"Empty response received!");

                        }
                    }
                    else
                        Console.WriteLine($"Failed to get data. Status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
