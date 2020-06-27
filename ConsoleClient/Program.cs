using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //discovery endpoint
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001/");

            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            //request access token 
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "console client",
                ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",
                Scope = "scope1"//  openid
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }


            //call identity Resource API
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);//设置一个head
            var response = await apiClient.GetAsync("http://localhost:5000/identity");//disco.UserInfoEndpoint
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }


            Console.ReadKey();
        }
    }
}
