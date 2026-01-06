using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SepoliaNftMarket.Models.DTO.Alchemy;

public class TokenUriConverter : JsonConverter<TokenUri>
{
    public override TokenUri? ReadJson(
        JsonReader reader,
        Type objectType,
        TokenUri? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return new();

        if (reader.TokenType == JsonToken.String)
        {
            var s = (string?)reader.Value ?? string.Empty;
            return new TokenUri { Raw = s, Gateway = string.Empty };
        }

        if (reader.TokenType == JsonToken.StartObject)
        {
            var obj = JObject.Load(reader);
            return new TokenUri
            {
                Raw = (string?)obj["raw"] ?? (string?)obj["uri"] ?? string.Empty,
                Gateway = (string?)obj["gateway"] ?? string.Empty
            };
        }
        
        throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing TokenIUri.");
    }

    public override void WriteJson(
        JsonWriter writer,
        TokenUri? value,
        JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }
        
        writer.WriteStartObject();
        writer.WritePropertyName("raw");
        writer.WriteValue(value.Raw);
        writer.WritePropertyName("gateway");
        writer.WriteValue(value.Gateway);
        writer.WriteEndObject();
    }
}