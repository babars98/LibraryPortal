using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace LibraryPortal.Util
{
    public class APIHelper
    {
        // Get Method for API
        public string GetMethod(string url)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = client.GetAsync(url).Result;

                //Check response status code for success
                if (response.IsSuccessStatusCode)
                    return response.Content.ReadAsStringAsync().Result;

            }
            catch (HttpRequestException e) { }

            return string.Empty;
        }

        public string PostMethod(string url, dynamic obj)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var data = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

                var response = client.PostAsync(url, data).Result;

                //Check response status code for success
                if (response.IsSuccessStatusCode)
                    return response.Content.ReadAsStringAsync().Result;

            }
            catch (HttpRequestException e) { }

            return string.Empty;
        }

        public string PutMethod(string url, dynamic obj)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var data = new StringContent(obj, Encoding.UTF8, "application/json");

                var response = client.PutAsync(url, data).Result;

                //Check response status code for success
                if (response.IsSuccessStatusCode)
                    return response.Content.ReadAsStringAsync().Result;

            }
            catch (HttpRequestException e) { }

            return string.Empty;
        }
    }
}
