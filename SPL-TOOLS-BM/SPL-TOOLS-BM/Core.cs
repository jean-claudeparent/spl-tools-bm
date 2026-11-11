using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NBitcoin;

namespace SPL_TOOLS_BM
{
    public class Core
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string SplinterlandsApiBase = "https://api2.splinterlands.com";

        public async Task<string> GetTokenAsync(string hiveAccount, string postingKey)
        {
            try
            {
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                string message = $"{hiveAccount}{timestamp}";

                // 1. Hacher le message brut en SHA-256 (sans préfixe Bitcoin)
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                byte[] hashBytes = SHA256.HashData(messageBytes);
                uint256 hash = new uint256(hashBytes);

                // 2. Charger la clé privée WIF Hive et générer une signature compacte (65 octets)
                BitcoinSecret secret = new BitcoinSecret(postingKey, Network.Main);
                CompactSignature compactSig = secret.PrivateKey.SignCompact(hash, forceLowR: true);

                // 3. Assembler l'octet de récupération (RecoveryId + 31 pour clé compressée) et les 64 octets (R + S)
                byte[] fullSig = new byte[65];
                fullSig[0] = (byte)(compactSig.RecoveryId + 31);
                Array.Copy(compactSig.Signature, 0, fullSig, 1, 64);

                // 4. Convertir en hexadécimal minuscule (format attendu par Splinterlands)
                string signatureHex = Convert.ToHexString(fullSig).ToLowerInvariant();

                // 5. Appel à l'API de connexion Splinterlands
                string url = $"{SplinterlandsApiBase}/players/login?name={Uri.EscapeDataString(hiveAccount)}&ts={timestamp}&sig={signatureHex}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseString = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(responseString))
                {
                    if (doc.RootElement.TryGetProperty("token", out JsonElement tokenElement))
                    {
                        return tokenElement.GetString();
                    }
                    throw new Exception("Connexion réussie, mais aucun token retourné.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération du token : {ex.Message}");
                return null;
            }
        }

        public string GetToken(string hiveAccount, string postingKey)
        {
            return GetTokenAsync(hiveAccount, postingKey).GetAwaiter().GetResult();
        }
    }
}
