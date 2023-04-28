using System.Security.Cryptography;
using System.Text;

namespace project_GameStore_server.Service
{
    internal class Extentions
    {
        public static string ComputeSha256Hash(string rawData)
        {
            // ComputeHash - returns byte array  
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));
            return new(bytes.SelectMany(x => x.ToString("x2").ToCharArray()).ToArray());                       
        }
    }
}
