using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Sistema_Bibliotecario_BIBLIO_UTEC.Models
{
    public static class SessionExtensions
    {
        // Guarda cualquier objeto serializándolo a JSON
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // Recupera el objeto deserializándolo desde JSON
        public static T? Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }

}