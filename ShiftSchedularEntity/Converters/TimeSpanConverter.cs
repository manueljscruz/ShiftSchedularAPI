using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShiftSchedularEntity.Converters
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string timeString = reader.GetString();

            if (TimeSpan.TryParse(timeString, out TimeSpan timeSpan))
            {
                return timeSpan;
            }

            throw new JsonException($"Invalid TimeSpan format: {timeString}");
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(@"hh\:mm"));
        }
    }
}
