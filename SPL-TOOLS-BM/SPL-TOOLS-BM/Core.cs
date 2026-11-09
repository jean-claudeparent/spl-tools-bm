using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using NBitcoin;




namespace SPL_TOOLS_BM
{
    public class Core
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string SplinterlandsApiBase = "https://api2.splinterlands.com";
        /// <summary>
        /// Authenticates with Splinterlands using a Hive Account and Private Posting Key 
        /// to retrieve a valid session token.
        /// </summary>




        public async Task<string> GetTokenAsync(string hiveAccount, string postingKey)
        {
            return  "not yet implenmeted";
           
        }

    }
}
