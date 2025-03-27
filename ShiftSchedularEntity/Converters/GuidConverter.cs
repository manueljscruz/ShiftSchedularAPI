using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace ShiftSchedularEntity.Converters
{
    public class GuidConverter : JsonConverter<Guid>
    {
        public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string guidString = HttpUtility.UrlDecode(reader.GetString());
            Guid guid = Guid.Empty;

            if (guidString == string.Empty)
                return guid;

            bool isValidGuid = Guid.TryParse(guidString, out guid);
            if (isValidGuid)
                return guid;
            else
            {
                byte[] bytes = Convert.FromBase64String(guidString);
                guid = new Guid(bytes);
            }
            return guid;
        }

        public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
