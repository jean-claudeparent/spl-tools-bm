using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace SPL_TOOLS_BM
{


    public class HiveKeyConverter
{
    private const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

    public static string CompressHiveWif(string wif)
    {
        // 1. Base58 decode (1 byte prefix 0x80 + 32 bytes key + 4 bytes checksum = 37 bytes)
        byte[] decoded = Base58Decode(wif);
        if (decoded.Length != 37 || decoded[0] != 0x80)
        {
            throw new ArgumentException("Invalid uncompressed WIF key (must be 51 characters long and start with '5').");
        }

        // Optional verification of the original checksum
        byte[] originalPayload = decoded.Take(33).ToArray();
        byte[] originalChecksum = decoded.Skip(33).Take(4).ToArray();
        if (!ComputeChecksum(originalPayload).SequenceEqual(originalChecksum))
        {
            throw new ArgumentException("Invalid original WIF key checksum.");
        }

        // 2. Append the 0x01 compression flag after the 32-byte private key (34 bytes total)
        byte[] extendedKey = new byte[34];
        Array.Copy(decoded, 0, extendedKey, 0, 33);
        extendedKey[33] = 0x01;

        // 3. Compute the new checksum (double SHA-256)
        byte[] newChecksum = ComputeChecksum(extendedKey);

        // 4. Final assembly (34 bytes + 4 bytes = 38 bytes) and Base58 encoding
        byte[] finalBytes = new byte[38];
        Array.Copy(extendedKey, 0, finalBytes, 0, 34);
        Array.Copy(newChecksum, 0, finalBytes, 34, 4);

        return Base58Encode(finalBytes);
    }

    private static byte[] ComputeChecksum(byte[] data)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash1 = sha256.ComputeHash(data);
            byte[] hash2 = sha256.ComputeHash(hash1);
            return hash2.Take(4).ToArray();
        }
    }

    private static byte[] Base58Decode(string input)
    {
        BigInteger intData = 0;
        foreach (char c in input)
        {
            int digit = Alphabet.IndexOf(c);
            if (digit < 0)
                throw new FormatException($"Invalid Base58 character: {c}");
            intData = intData * 58 + digit;
        }

        // BigInteger.ToByteArray() is Little-Endian and may append a 0x00 sign byte
        byte[] bytes = intData.ToByteArray();
        if (bytes.Length > 1 && bytes[bytes.Length - 1] == 0)
        {
            bytes = bytes.Take(bytes.Length - 1).ToArray();
        }
        Array.Reverse(bytes); // Convert to Big-Endian

        // Handle leading '1's (representing 0x00 bytes)
        int leadingZeros = input.TakeWhile(c => c == '1').Count();
        byte[] result = new byte[leadingZeros + bytes.Length];
        Array.Copy(bytes, 0, result, leadingZeros, bytes.Length);

        return result;
    }

    private static string Base58Encode(byte[] data)
    {
        // Append a 0x00 byte at the end (Little-Endian) to force BigInteger to be positive
        byte[] reversed = data.Reverse().Concat(new byte[] { 0x00 }).ToArray();
        BigInteger intData = new BigInteger(reversed);

        StringBuilder result = new StringBuilder();
        while (intData > 0)
        {
            intData = BigInteger.DivRem(intData, 58, out BigInteger remainder);
            result.Insert(0, Alphabet[(int)remainder]);
        }

        // Preserve leading 0x00 bytes as '1's
        foreach (byte b in data)
        {
            if (b == 0)
                result.Insert(0, '1');
            else
                break;
        }

        return result.ToString();
    }
}
}

// Usage example:
// string compressedWif = HiveKeyConverter.CompressHiveWif("5J...");
