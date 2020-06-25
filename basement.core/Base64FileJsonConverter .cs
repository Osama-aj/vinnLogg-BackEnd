using Newtonsoft.Json;
using System;

namespace basement.core
{
    public class Base64FileJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }



        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return Convert.FromBase64String(reader.Value as string);
        }

        //not used
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //writer.WriteValue(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes((string)value)));
            throw new NotImplementedException();
        }
    }
}