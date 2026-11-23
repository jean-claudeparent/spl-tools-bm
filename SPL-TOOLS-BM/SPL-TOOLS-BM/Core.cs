using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using NBitcoin;
using System.IO;


namespace SPL_TOOLS_BM
{
    public class Core
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string SplinterlandsApiBase = "https://api2.splinterlands.com";
        public string OutputFolder = @"c:\devcenter\data\spl-tools-bm\";
        

        public async Task<string> GetTokenAsync(string hiveAccount, string postingKey)
        {
            try
            {
                if ((postingKey.Length  == 51) && (postingKey [0] == '5'))                    {
                    // the key is not compressed, compress it
                    postingKey  = HiveKeyConverter.CompressHiveWif(postingKey );



                        }
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                string message = $"{hiveAccount}{timestamp}";

                // 1. Hach the raw message in SHA-256 (without  Bitcoin prefix)
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                byte[] hashBytes = SHA256.HashData(messageBytes);
                uint256 hash = new uint256(hashBytes);

                // 2. Load' the private key in WIF Hive format and génerate a signature compressed (65 bytes)
                BitcoinSecret secret = new BitcoinSecret(postingKey, Network.Main);
                CompactSignature compactSig = secret.PrivateKey.SignCompact(hash, forceLowR: true);

                // 3. Assemble the byte of recovery (RecoveryId + 31 for compress key) and the 64 bytes
                byte[] fullSig = new byte[65];
                fullSig[0] = (byte)(compactSig.RecoveryId + 31);
                Array.Copy(compactSig.Signature, 0, fullSig, 1, 64);

                // 4. convert to hex lowercase (the format that splinterlands use)
                string signatureHex = Convert.ToHexString(fullSig).ToLowerInvariant();

                // 5. Call the connection api for splinterlands
                string url = $"{SplinterlandsApiBase}/players/v2/login?name={Uri.EscapeDataString(hiveAccount)}&ts={timestamp}&sig={signatureHex}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseString = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(responseString))
                {
                    if (doc.RootElement.TryGetProperty("token", out JsonElement tokenElement))
                    {
                        return tokenElement.GetString();
                    }
                    throw new Exception("s01 Connection was successfull but the token has a problem : json=" +  JsonSerializer.Serialize(doc));
                }
            }
            catch (Exception ex)
            {
                 
                throw new Exception("s02 Error while getting the token : " + ex.Message );

            }
        }

        public string GetToken(string hiveAccount, string postingKey)
        {
            return GetTokenAsync(hiveAccount, postingKey).GetAwaiter().GetResult();
        }
        public async Task<string> GetBattlesAsync(string Token, string Player)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Player))
                {
                    throw new ArgumentException("Player name cannot be null or empty.");
                }

                if (!Directory.Exists(OutputFolder))
                {
                    Directory.CreateDirectory(OutputFolder);
                }

                // Build the request URL with player, username, and authentication token
                string url = $"{SplinterlandsApiBase}/battle/history?player={Uri.EscapeDataString(Player)}";

                if (!string.IsNullOrWhiteSpace(Token))
                {
                    url += $"&username={Uri.EscapeDataString(Player)}&token={Uri.EscapeDataString(Token)}";
                }

                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Save the raw JSON response to your OutputFolder
                string fileName = $"battles_{Player}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
                string filePath = Path.Combine(OutputFolder, fileName);
                await File.WriteAllTextAsync(filePath, jsonResponse, Encoding.UTF8);

                return jsonResponse;
            }
            catch (Exception ex)
            {
                throw new Exception($"s03 Error while getting battles for {Player}: {ex.Message}");
            }
        }

        public static string FormatJson (string  rawawjson)
        {
            using JsonDocument doc = JsonDocument.Parse(rawawjson);
            return  JsonSerializer.Serialize(doc, new JsonSerializerOptions
            {
                WriteIndented = true
            });


        }

    } // end class
    }  // end namespace
