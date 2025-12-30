using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace ShiftSchedularEntity.Converters
{
    public class GuidConverter : JsonConverter<Guid>
    {
        public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("Expected string for Guid value.");

            string value = reader.GetString();

            if (string.IsNullOrWhiteSpace(value))
                return Guid.Empty;

            // STEP 1: Normalize URL-encoded values ONLY if encoding is present
            // '%' never appears in valid Base64 or standard Guid strings
            if (value.Contains('%'))
            {
                value = Uri.UnescapeDataString(value);
            }

            // STEP 2: Standard Guid format
            if (Guid.TryParse(value, out Guid guid))
            {
                return guid;
            }

            // STEP 3: Base64-encoded binary Guid
            byte[] bytes;

            try
            {
                bytes = Convert.FromBase64String(value);
            }
            catch (FormatException ex)
            {
                throw new JsonException(
                    $"Invalid Guid value. Expected Guid or Base64-encoded Guid, got: '{value}'",
                    ex);
            }

            if (bytes.Length != 16)
                throw new JsonException("Invalid Base64 Guid length.");

            return new Guid(bytes);
        }


        //public override Guid _Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        //{
        //    string guidString = HttpUtility.UrlDecode(reader.GetString());
        //    Guid guid = Guid.Empty;

        //    if (guidString == string.Empty)
        //        return guid;

        //    bool isValidGuid = Guid.TryParse(guidString, out guid);
        //    if (isValidGuid)
        //        return guid;
        //    else
        //    {
        //        byte[] bytes = Convert.FromBase64String(guidString);
        //        guid = new Guid(bytes);
        //    }
        //    return guid;
        //}

        public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
