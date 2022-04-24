using System.IO;
using System.Text.Json;

namespace Notification.Consumer.Utils.Helper
{
    public static class ByteHelper
    {
        public static T ByteArrayToObject<T>(byte[] data) where T : class
        {
            using MemoryStream ms = new();
            ms.Write(data, 0, data.Length);
            ms.Seek(0, SeekOrigin.Begin);
            return JsonSerializer.DeserializeAsync<T>(ms).Result;
        }
    }
}
