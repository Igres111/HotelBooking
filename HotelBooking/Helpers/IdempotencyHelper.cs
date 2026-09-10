using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace HotelBooking.Helpers
{
    public static class IdempotencyHelper
    {
        public static string ComputeRequestHash<TRequest>(TRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var bytes = Encoding.UTF8.GetBytes(json);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash);
        }
    }
}
