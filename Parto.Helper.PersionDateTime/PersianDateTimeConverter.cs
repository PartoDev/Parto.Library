using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Parto.Helper.PersionDateTime
{
    public class PersianDateTimeConverter : JsonConverter<PersianDateTime>
    {
        public override PersianDateTime Read(ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
            new(reader.GetString());

        public override void Write(Utf8JsonWriter writer, PersianDateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}