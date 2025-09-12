using System.Text.Json;
using System.Text.Json.Serialization;
using IntegracaoDevOps.Data.DTOs;

namespace IntegracaoDevOps.Converters;

public class UserInfoConverter : JsonConverter<UserInfo>
{
    public override UserInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            return new UserInfo { DisplayName = reader.GetString() };
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            return JsonSerializer.Deserialize<UserInfo>(ref reader, options);
        }

        reader.Skip();
        return null;
    }

    public override void Write(Utf8JsonWriter writer, UserInfo value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}